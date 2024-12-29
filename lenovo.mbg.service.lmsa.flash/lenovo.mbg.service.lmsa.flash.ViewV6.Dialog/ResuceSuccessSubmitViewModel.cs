using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class ResuceSuccessSubmitViewModel : NotifyBase
{
	private Brush normalBrush;

	private Brush errorBrush;

	private long interlocker;

	private string modelName;

	private string beforeText;

	private string afterText;

	private bool uplodaLogs = true;

	private bool submitButtonIsEnabled = true;

	private Brush beforeBorderBrush;

	private Brush afterBorderBrush;

	public string BeforeText
	{
		get
		{
			return beforeText;
		}
		set
		{
			beforeText = value;
			if (!string.IsNullOrEmpty(value))
			{
				BeforeBorderBrush = normalBrush;
			}
			OnPropertyChanged("BeforeText");
		}
	}

	public string AfterText
	{
		get
		{
			return afterText;
		}
		set
		{
			afterText = value;
			if (!string.IsNullOrEmpty(value))
			{
				AfterBorderBrush = normalBrush;
			}
			OnPropertyChanged("AfterText");
		}
	}

	public bool UplodaLogs
	{
		get
		{
			return uplodaLogs;
		}
		set
		{
			uplodaLogs = value;
			OnPropertyChanged("UplodaLogs");
		}
	}

	public bool SubmitButtonIsEnabled
	{
		get
		{
			return submitButtonIsEnabled;
		}
		set
		{
			submitButtonIsEnabled = value;
			OnPropertyChanged("SubmitButtonIsEnabled");
		}
	}

	public Brush BeforeBorderBrush
	{
		get
		{
			return beforeBorderBrush;
		}
		set
		{
			beforeBorderBrush = value;
			OnPropertyChanged("BeforeBorderBrush");
		}
	}

	public Brush AfterBorderBrush
	{
		get
		{
			return afterBorderBrush;
		}
		set
		{
			afterBorderBrush = value;
			OnPropertyChanged("AfterBorderBrush");
		}
	}

	public ResuceSuccessSubmitViewModel(string modelName)
	{
		this.modelName = modelName;
		normalBrush = Application.Current.TryFindResource("V6_BorderBrushKey") as SolidColorBrush;
		errorBrush = Application.Current.TryFindResource("V6_WarnningBrushKey") as SolidColorBrush;
		beforeBorderBrush = normalBrush;
		afterBorderBrush = normalBrush;
	}

	public void ReleaseLocker()
	{
		if (Interlocked.Read(ref interlocker) != 0L)
		{
			Interlocked.Exchange(ref interlocker, 0L);
		}
	}

	public async Task<bool?> Submit()
	{
		if (Interlocked.Read(ref interlocker) != 0L)
		{
			return null;
		}
		try
		{
			Interlocked.Exchange(ref interlocker, 1L);
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				if (string.IsNullOrEmpty(BeforeText))
				{
					BeforeBorderBrush = errorBrush;
				}
				if (string.IsNullOrEmpty(AfterText))
				{
					AfterBorderBrush = errorBrush;
				}
			});
			if (string.IsNullOrEmpty(BeforeText) || string.IsNullOrEmpty(AfterText))
			{
				return null;
			}
			List<string> list = new List<string>();
			string logPath = null;
			if (UplodaLogs)
			{
				logPath = GetLogs();
				string browserLog = GetBrowserLog();
				list.Add(logPath);
				list.Add(browserLog);
			}
			bool value = await SubmitAsync(list);
			GlobalFun.TryDeleteFile(logPath);
			return value;
		}
		catch
		{
			return false;
		}
		finally
		{
			Interlocked.Exchange(ref interlocker, 0L);
		}
	}

	private string GetLogs()
	{
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
		string text = DateTime.Now.ToString("yyyy-MM-dd");
		string encryptFile = Path.Combine(path, text + ".log.dpapi");
		LogAesDecrypt logAesDecrypt = new LogAesDecrypt();
		string text2 = Path.Combine(path, text + ".decrpyt.log");
		if (logAesDecrypt.Decrypt2File(encryptFile, text2))
		{
			return text2;
		}
		return null;
	}

	private string GetBrowserLog()
	{
		string text = Path.Combine(Path.GetTempPath(), "lmsatemp");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		else
		{
			GlobalFun.DeleteFileInDirectory(text);
		}
		string chromiumLogFilePath = Configurations.ChromiumLogFilePath;
		string text2 = Path.Combine(text, DateTime.Now.ToString("yyyy-MM-dd") + "-browser.log");
		File.Copy(chromiumLogFilePath, text2, overwrite: true);
		return text2;
	}

	public async Task<bool> SubmitAsync(List<string> files, int tryIndex = 0)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["clientVersion"] = WebApiContext.CLIENT_VERSION;
		dictionary.Add("rescueBeforeDesc", BeforeText);
		dictionary.Add("rescueAfterDesc", AfterText);
		dictionary.Add("modelName", modelName);
		dictionary.Add("windowsInfo", WebApiContext.WINDOWS_VERSION);
		return await new ApiService().UploadAsync(WebApiUrl.COLLECTION_RESCUE_SUCCESS_LOG_UPLOAD, files, dictionary);
	}
}
