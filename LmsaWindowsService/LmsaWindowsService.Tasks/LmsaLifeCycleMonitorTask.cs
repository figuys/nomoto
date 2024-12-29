using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using LmsaWindowsService.Common;
using LmsaWindowsService.Contexts;
using LmsaWindowsService.Contracts;
using LmsaWindowsService.PipeWorkers;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace LmsaWindowsService.Tasks;

public class LmsaLifeCycleMonitorTask : ITask, IDisposable
{
	protected Timer monitorTimer;

	private const int PERIOD = 60000;

	private const string PROCESSNAME = "Software Fix";

	protected static string REGISTRY_KEY = "Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\LmsaNtService.exe";

	private Process _LmsaProcess;

	private bool _Checked;

	private bool _Upload;

	public bool IsRunning { get; private set; }

	public string Name { get; private set; }

	public LmsaLifeCycleMonitorTask()
		: this(typeof(LmsaLifeCycleMonitorTask).Name)
	{
	}

	public LmsaLifeCycleMonitorTask(string taskName)
	{
		Name = taskName;
		PipeLmsaUploadConfirm.OnConfirm += PipeLmsaUploadConfirmHandler;
		PipeLmsaRunning.OnNotify += PiprLmsaRunningHandler;
	}

	private void PiprLmsaRunningHandler(object sender, object e)
	{
		Process[] processesByName = Process.GetProcessesByName("Software Fix");
		if (processesByName.Length != 0 && _LmsaProcess == null)
		{
			_LmsaProcess = processesByName[0];
			_LmsaProcess.EnableRaisingEvents = true;
			_LmsaProcess.Exited += LmsaProcessExitedHandler;
		}
	}

	public void Start()
	{
		IsRunning = true;
	}

	public void Stop()
	{
		Dispose();
	}

	public void Dispose()
	{
		IsRunning = false;
	}

	private void MonitorCallback(object state)
	{
		Process[] processesByName = Process.GetProcessesByName("Software Fix");
		if (processesByName.Length != 0 && _LmsaProcess == null)
		{
			_LmsaProcess = processesByName[0];
			_LmsaProcess.EnableRaisingEvents = true;
			_LmsaProcess.Exited += LmsaProcessExitedHandler;
		}
	}

	private void LmsaProcessExitedHandler(object sender, EventArgs e)
	{
		Task.Run(delegate
		{
			GlobalFun.ClearFlashToolDirectory();
		});
		if (_LmsaProcess.ExitCode != 0)
		{
			ProcessCrushLog();
		}
		else
		{
			ProcessBackuprestoreLog();
		}
		PipeLmsaData.DataDic = null;
		_LmsaProcess.Exited -= LmsaProcessExitedHandler;
		_LmsaProcess.Close();
		_LmsaProcess.Dispose();
		_LmsaProcess = null;
	}

	private void ProcessCrushLog()
	{
		RegisterHelper.TryGetRegistryKey(Registry.LocalMachine, REGISTRY_KEY, "defaultupload", out var value);
		if (value != 1)
		{
			ProcessExtensions.StartProcessAsCurrentUser(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LMSA\\LmsaServiceUI\\LmsaServiceUI.exe"));
			if (SpinWait.SpinUntil(() => _Upload, 5000))
			{
				value = 1;
				if (_Checked)
				{
					RegisterHelper.WriteRegistry(REGISTRY_KEY, "defaultupload", 1);
				}
			}
			_Upload = false;
		}
		if (value == 1)
		{
			string logFile = GetLogFile();
			if (!string.IsNullOrEmpty(logFile))
			{
				TryAppendEventLogs(logFile);
				Upload(null, logFile);
			}
		}
	}

	private void ProcessBackuprestoreLog()
	{
		if (PipeLmsaData.DataDic == null)
		{
			return;
		}
		LogHelper.LogInstance.Info("ProcessBackuprestoreLog start");
		PipeLmsaData.DataDic.TryGetValue("BackupRestoreHasFailed", out var value);
		bool num = value != null && (bool)value;
		PipeLmsaData.DataDic.TryGetValue("BACK_UP_UPLOAD_FILE", out var value2);
		bool flag = num && value2 != null && value2.ToString().Equals("Y", StringComparison.CurrentCultureIgnoreCase);
		LogHelper.LogInstance.Info($"ProcessBackuprestoreLog isupload {flag}");
		if (flag)
		{
			string logFile = GetLogFile();
			LogHelper.LogInstance.Info("ProcessBackuprestoreLog logpath " + logFile);
			if (!string.IsNullOrEmpty(logFile))
			{
				Upload(new Dictionary<string, string> { { "type", "BACK_UP_UPLOAD_FILE" } }, logFile);
			}
		}
	}

	private async void Upload(Dictionary<string, string> dic, string logpath)
	{
		if (dic == null)
		{
			dic = new Dictionary<string, string>();
		}
		dic.Add("version", GetVeresion());
		dic.Add("windowsInfo", LMSAContext.OsVersionName);
		dic.Add("crashTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
		PipeLmsaData.DataDic.TryGetValue("guid", out var value);
		PipeLmsaData.DataDic.TryGetValue("jwt", out var value2);
		Dictionary<string, string> authorHeader = new Dictionary<string, string>();
		if (value != null && !string.IsNullOrEmpty(value.ToString()))
		{
			authorHeader.Add("guid", value.ToString());
		}
		if (value2 != null && !string.IsNullOrEmpty(value2.ToString()))
		{
			authorHeader.Add("Authorization", $"Bearer {value2}");
		}
		int trycount = 3;
		do
		{
			LogHelper.LogInstance.Debug($"crash upload params: {JsonHelper.SerializeObject2Json(dic)}, author header: {authorHeader}");
			bool flag = await HttpUploadFile.Upload(Context.WebAPI.UPLOAD_URL, logpath, dic, 0, 0, authorHeader);
			LogHelper.LogInstance.Info($"ProcessBackuprestoreLog upload status {flag}");
			if (flag)
			{
				break;
			}
			Thread.Sleep(300000);
		}
		while (trycount-- > 0);
		try
		{
			if (File.Exists(logpath))
			{
				File.SetAttributes(logpath, FileAttributes.Normal);
				File.Delete(logpath);
			}
		}
		catch (Exception)
		{
		}
	}

	protected string GetLogFile()
	{
		string text = Path.Combine(ServiceContext.Appdirectory, "logs");
		string text2 = DateTime.Now.ToString("yyyy-MM-dd");
		string encryptFile = Directory.GetFiles(text, text2 + ".log.dpapi", SearchOption.TopDirectoryOnly).FirstOrDefault();
		LogAesDecrypt logAesDecrypt = new LogAesDecrypt();
		string text3 = Path.Combine(text, text2 + ".decrpyt.log");
		if (logAesDecrypt.Decrypt2File(encryptFile, text3))
		{
			return text3;
		}
		return null;
	}

	protected string GetVeresion()
	{
		return LMSAContext.MainProcessVersion;
	}

	protected JObject GetUserName()
	{
		string text = FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "LatestLoginUserInfo");
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return JsonHelper.DeserializeJson2Jobjcet(text);
	}

	private void PipeLmsaUploadConfirmHandler(object sender, object e)
	{
		_Upload = true;
		_Checked = (bool)e;
	}

	private void TryAppendEventLogs(string filePath)
	{
		try
		{
			EventLogEntryCollection entries = new EventLog
			{
				Log = "Application"
			}.Entries;
			if (entries == null || entries.Count <= 0)
			{
				return;
			}
			using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write);
			fileStream.Seek(0L, SeekOrigin.End);
			DateTime dateTime = DateTime.Now.AddHours(-1.0);
			foreach (EventLogEntry item in entries)
			{
				if (item.EntryType == EventLogEntryType.Error && item.TimeGenerated >= dateTime)
				{
					byte[] bytes = Encoding.UTF8.GetBytes("-------------------------Windows Event Logs Begin---------------------------" + Environment.NewLine + Environment.NewLine);
					fileStream.Write(bytes, 0, bytes.Length);
					bytes = Encoding.UTF8.GetBytes(item.Message + Environment.NewLine);
					fileStream.Write(bytes, 0, bytes.Length);
					bytes = Encoding.UTF8.GetBytes("-------------------------Windows Event Logs End---------------------------" + Environment.NewLine + Environment.NewLine);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			fileStream.Flush();
		}
		catch (Exception)
		{
		}
	}
}
