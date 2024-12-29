using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.smartdevice.ODMSocketServer;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class Shell : BaseStep
{
	protected int timeout = 1200000;

	protected bool IsConnected;

	private bool mError;

	private bool mCompleted;

	protected bool quit;

	private string failedResponse;

	private int terminatedRetry = 2;

	private ShellResponse.ShellCmdStatus ToolErrorStatus;

	private AutoResetEvent autoLockHandler;

	private DateTime startTime;

	private List<string> decryptFiles;

	private int responseCount;

	private int responseTriggerCount;

	private int logMonitorType;

	private bool autoCloseWhenGoOnResponse;

	private bool deviceMonitorRunning;

	private string QuitConditionMessage;

	private bool IsRetry;

	private Task ConnectTutorialsTask;

	private const string FileLostString = "No such file or directory";

	private readonly List<JObject> QuitConditionList = new List<JObject>
	{
		new JObject
		{
			{
				"Condition",
				new JArray("No such file or directory")
			},
			{ "Message", null }
		}
	};

	public override void Run()
	{
		if (base.TimeoutMilliseconds <= 0)
		{
			base.TimeoutMilliseconds = timeout;
		}
		if (Retry > 0)
		{
			terminatedRetry = Retry;
		}
		if (base.Info.Args.QuitConditions is JArray { HasValues: not false } jArray)
		{
			foreach (JObject item in jArray)
			{
				QuitConditionList.Add(item);
			}
		}
		List<Tuple<string, string, ShellResponse>> list = Init();
		if (ToolErrorStatus == ShellResponse.ShellCmdStatus.FileLostError)
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, failedResponse);
			return;
		}
		Result result = Result.PASSED;
		dynamic val = ((base.Info.Args.FlashRetry == null) ? ((object)0) : base.Info.Args.FlashRetry.Value);
		Task<List<string>> task = StartDeviceMonitorTask();
		do
		{
			string text = null;
			startTime = DateTime.Now;
			foreach (Tuple<string, string, ShellResponse> item2 in list)
			{
				result = DoFlash(item2.Item1, item2.Item2, item2.Item3);
			}
			IsRetry = false;
			switch (result)
			{
			case Result.FAILED:
				if (ToolErrorStatus == ShellResponse.ShellCmdStatus.FastbootError || ToolErrorStatus == ShellResponse.ShellCmdStatus.FileLostError)
				{
					break;
				}
				if (!IsConnected && Retry > 0)
				{
					if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("ReSteps") != null)
					{
						Retry--;
						IsRetry = true;
					}
					text = base.Info.Args.ConnectTutorials?.RetryText ?? base.Info.Args.ConnectSteps?.RetryText ?? base.Info.Args.ReconnectPromptText;
					if (!IsRetry && text != null)
					{
						Retry--;
						base.Log.AddLog("device not connected, will try again", upload: true);
						IsRetry = true;
					}
				}
				else if (IsConnected && list.Select((Tuple<string, string, ShellResponse> n) => n.Item3).Count((ShellResponse n) => n.ShellCmd == ShellResponse.ShellCmdType.CmdDloader || n.ShellCmd == ShellResponse.ShellCmdType.CmdDloaderTablet) > 0 && val > 0)
				{
					text = base.Info.Args.ConnectTutorials?.FlashRetryText ?? base.Info.Args.ConnectSteps?.FlashRetryText;
					if (text != null)
					{
						dynamic val2 = val;
						val = --val2;
						base.Log.AddLog("Unisoc device rescue failed, will try again", upload: true);
						IsRetry = true;
					}
				}
				break;
			case Result.SHELL_EXE_TERMINATED_EXIT:
			case Result.SHELL_EXE_START_FAILED:
				if (terminatedRetry-- > 0)
				{
					IsRetry = true;
					Thread.Sleep(20000);
				}
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				int num = (int)((double)base.TimeoutMilliseconds - DateTime.Now.Subtract(startTime).TotalMilliseconds);
				if (num < 10000)
				{
					num = 10000;
				}
				if (!base.Recipe.UcDevice.MessageBox.Show(base.Info.Name, text).Wait(num))
				{
					base.Recipe.UcDevice.MessageBox.Close(true);
				}
			}
		}
		while (IsRetry);
		switch (result)
		{
		case Result.FAILED:
			result = ((ToolErrorStatus == ShellResponse.ShellCmdStatus.RomUnMatchError) ? Result.ROM_UNMATCH_FAILED : ((ToolErrorStatus == ShellResponse.ShellCmdStatus.AuthorizedError) ? Result.AUTRORIZED_FAILED : ((ToolErrorStatus == ShellResponse.ShellCmdStatus.FastbootError) ? Result.SHELL_RESCUE_FAILED : ((!IsConnected) ? Result.SHELL_CONNECTED_FAILED : (CheckComport() ? Result.SHELL_RESCUE_FAILED : Result.PROCESS_FORCED_TEREMINATION)))));
			if (string.IsNullOrEmpty(failedResponse))
			{
				failedResponse = "shell execute timeout";
			}
			if (list.Select((Tuple<string, string, ShellResponse> n) => n.Item3).Count((ShellResponse n) => n.ShellCmd == ShellResponse.ShellCmdType.MTekCfcFlashTool) > 0)
			{
				List<string> values = ProcessRunner.ProcessList(LoadToolPath("fastboot.exe"), EncapsulationFastbootCommand("getvar all"), 5000);
				base.Log.AddLog("command : getvar all, response: " + string.Join("\r\n", values), upload: true);
			}
			break;
		case Result.SHELL_EXE_TERMINATED_EXIT:
		case Result.SHELL_EXE_START_FAILED:
		{
			failedResponse = ((result == Result.SHELL_EXE_START_FAILED) ? "The tool process has not started" : "The tool process terminated abnormally");
			base.Recipe.UcDevice.MessageBox.Show("K0071", "K1832", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
			string text2 = base.Resources.Get(RecipeResources.TooL);
			if (!Directory.Exists(text2))
			{
				base.Log.AddLog("tool path: " + text2 + " not exists", upload: true);
				break;
			}
			List<string> allFiles = GlobalFun.GetAllFiles(text2);
			if (allFiles != null && allFiles.Count > 0)
			{
				base.Log.AddLog($"tool path: {text2}, total files: {allFiles.Count}", upload: true);
				foreach (string item3 in allFiles)
				{
					FileInfo fileInfo = new FileInfo(item3);
					base.Log.AddLog($"{fileInfo.FullName}, {fileInfo.Length}, {fileInfo.LastWriteTime}", upload: true);
				}
			}
			else
			{
				base.Log.AddLog("tool path: " + text2 + ", total files: 0", upload: true);
			}
			break;
		}
		}
		FreeLock();
		base.Recipe.UcDevice.MessageBox.Close(true);
		ODMServerMain.CloseAllSockets();
		if (result != Result.PASSED)
		{
			deviceMonitorRunning = false;
			base.Log.AddLog("device connect monitor: " + Environment.NewLine + string.Join("\r\n", task.Result), upload: true);
		}
		if (decryptFiles != null && decryptFiles.Count > 0)
		{
			Task.Run(delegate
			{
				decryptFiles.ForEach(delegate(string n)
				{
					GlobalFun.TryDeleteFile(n);
				});
			});
		}
		if (!string.IsNullOrEmpty(QuitConditionMessage))
		{
			base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, QuitConditionMessage, null, new List<string> { "K0327" }, 10000, null, showClose: false, popupWhenClose: false, format: true, true);
		}
		base.Log.AddResult(this, result, failedResponse);
	}

	private List<Tuple<string, string, ShellResponse>> Init()
	{
		string category = base.Resources.Get("category");
		string text = base.Info.Args.EXE;
		string text2 = base.Info.Args.Command;
		List<Tuple<string, string, ShellResponse>> list = new List<Tuple<string, string, ShellResponse>>();
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			List<object> list2 = new List<object>();
			if (base.Info.Args.Format != null)
			{
				foreach (object item5 in base.Info.Args.Format)
				{
					string text3 = (string)(dynamic)item5;
					object item = text3;
					if (text3.StartsWith("$"))
					{
						string key2 = text3.Substring(1);
						item = base.Cache[key2];
					}
					list2.Add(item);
				}
				text2 = string.Format(text2, list2.ToArray());
			}
			string value = Regex.Match(text2, "\"(?<key>.*scatter.*\\.txt)").Groups["key"].Value;
			if (File.Exists(value))
			{
				base.Log.AddLog("check sactter file: " + value, upload: true);
				MatchCollection matchCollection = Regex.Matches(File.ReadAllText(value), "(?<key>file_name):\\s+(?<value>.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				string path = base.Resources.Get(RecipeResources.Rom);
				foreach (Match item6 in matchCollection)
				{
					string value2 = item6.Groups["value"].Value;
					if (!string.IsNullOrEmpty(value2) && !"NONE".Equals(value2, StringComparison.CurrentCultureIgnoreCase))
					{
						string text4 = Path.Combine(path, value2);
						if (!File.Exists(text4))
						{
							failedResponse = text4 + " not exists";
							ToolErrorStatus = ShellResponse.ShellCmdStatus.FileLostError;
							return list;
						}
					}
				}
			}
			ShellResponse item2 = new ShellResponse(text, list2, category);
			list.Add(new Tuple<string, string, ShellResponse>(text, text2, item2));
		}
		else
		{
			string text5 = base.Info.Args.ComPort ?? string.Empty;
			string name = base.Info.Args.StartupFile ?? "Flash.cmd";
			string localFilePath = base.Resources.GetLocalFilePath("toolFolder");
			if (!string.IsNullOrEmpty(text5))
			{
				string key3 = (base.Cache.ContainsKey(text5) ? text5 : "comport");
				object arg = base.Cache[key3];
				text5 = $"COM{arg}";
			}
			string recoveryCmd = base.Resources.GetRecoveryCmd(name);
			if (string.IsNullOrWhiteSpace(recoveryCmd))
			{
				ToolErrorStatus = ShellResponse.ShellCmdStatus.FileLostError;
				failedResponse = "Recipe Shell StartupFile doesn't exist!";
				return list;
			}
			string[] array = Regex.Replace(Regex.Replace(File.ReadAllText(recoveryCmd).Trim(), "%~dp0", Path.GetDirectoryName(recoveryCmd) + "\\", RegexOptions.IgnoreCase).Trim(), "pause", string.Empty, RegexOptions.IgnoreCase).Trim().Replace(".\\portname", ".\\" + text5)
				.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text6 in array)
			{
				string text7 = text6.Substring(0, text6.IndexOf(' '));
				string item3 = text6.Substring(text6.IndexOf(' '));
				string[] files = Directory.GetFiles(localFilePath, text7, SearchOption.AllDirectories);
				if (files.Length == 0)
				{
					ToolErrorStatus = ShellResponse.ShellCmdStatus.FileLostError;
					failedResponse = "Recipe Shell FlashToolExe:[" + text7 + "] doesn't exist!";
					return list;
				}
				ShellResponse item4 = new ShellResponse(files[0], null, category);
				list.Add(new Tuple<string, string, ShellResponse>(files[0], item3, item4));
			}
			try
			{
				string text8 = base.Info.Args.DecryptFileType;
				if (!string.IsNullOrEmpty(text8))
				{
					decryptFiles = GlobalFun.DecryptRomFile(base.Resources.Get("Rom"), text8);
				}
			}
			catch (Exception)
			{
			}
		}
		return list;
	}

	protected Result DoFlash(string exe, string command, ShellResponse shellResponse)
	{
		Result result = Result.PASSED;
		IsConnected = false;
		mCompleted = false;
		mError = false;
		quit = false;
		ConnectTutorialsTask = null;
		responseCount = 0;
		failedResponse = null;
		bool flag = true;
		bool flag2 = false;
		ToolErrorStatus = ShellResponse.ShellCmdStatus.None;
		Process process = new Process();
		process.StartInfo.FileName = "\"" + exe + "\"";
		process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exe);
		process.StartInfo.Arguments = command;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.Verb = "runas";
		process.EnableRaisingEvents = true;
		process.StartInfo.CreateNoWindow = true;
		process.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
		{
			Redirected(e, shellResponse);
		};
		process.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
		{
			Redirected(e, shellResponse);
		};
		try
		{
			process.Start();
		}
		catch (Exception arg)
		{
			base.Log.AddLog($"exe: {exe}, command {command}, exeption: {arg}", upload: true);
			return Result.SHELL_EXE_START_FAILED;
		}
		base.Log.AddLog($"exe: {exe}, pid: {process.Id}, command {command}", upload: true);
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		if (base.Info.Args.Input != null)
		{
			string text = base.Info.Args.Input;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			if (text != string.Empty)
			{
				process.StandardInput.WriteLine(text);
			}
		}
		if (base.Info.Args.ShowRescuingMask == true)
		{
			base.Recipe.FreeEventHandler(realFlash: true);
		}
		if (shellResponse.ShellCmd != ShellResponse.ShellCmdType.CmdDloader && shellResponse.ShellCmd != ShellResponse.ShellCmdType.CmdDloaderTablet && shellResponse.ShellCmd != ShellResponse.ShellCmdType.MTekFlashTool && shellResponse.ShellCmd != ShellResponse.ShellCmdType.MTekSpFlashTool)
		{
			Task.Run(() => ShowMessageWhenStartToolAsync(shellResponse));
		}
		do
		{
			if (mCompleted || mError || quit || process.HasExited)
			{
				flag = false;
				break;
			}
			Thread.Sleep(1000);
		}
		while (DateTime.Now.Subtract(startTime).TotalMilliseconds < (double)base.TimeoutMilliseconds);
		process.EnableRaisingEvents = false;
		process.OutputDataReceived -= delegate(object s, DataReceivedEventArgs e)
		{
			Redirected(e, shellResponse);
		};
		process.OutputDataReceived -= delegate(object s, DataReceivedEventArgs e)
		{
			Redirected(e, shellResponse);
		};
		if (!process.HasExited)
		{
			process.Kill();
		}
		Thread.Sleep(3000);
		if (quit)
		{
			result = ((ToolErrorStatus == ShellResponse.ShellCmdStatus.FileLostError) ? Result.LOAD_RESOURCE_FAILED : ((ToolErrorStatus == ShellResponse.ShellCmdStatus.FastbootDegrade) ? Result.FASTBOOT_DEGRADE_QUIT : ((ToolErrorStatus != ShellResponse.ShellCmdStatus.ConditionQuit) ? Result.MANUAL_QUIT : Result.INTERCEPTOR_QUIT)));
		}
		else if (!mCompleted && !mError && !quit && !flag)
		{
			flag2 = true;
			result = Result.SHELL_EXE_TERMINATED_EXIT;
		}
		else if (mError || !mCompleted || flag)
		{
			result = Result.FAILED;
		}
		base.Log.AddLog($"shell completed: {mCompleted}, error: {mError}, quit: {quit}, terminated exit: {flag2}, timeout-{base.TimeoutMilliseconds}: {flag}", upload: true);
		FreeLock();
		base.Recipe.UcDevice.MessageBox.Close(true);
		return result;
	}

	private static void KillProcessAndChildren(int pid)
	{
		if (pid == 0)
		{
			return;
		}
		foreach (ManagementObject item in new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid).Get())
		{
			KillProcessAndChildren(System.Convert.ToInt32(item["ProcessID"]));
		}
		try
		{
			Process.GetProcessById(pid).Kill();
		}
		catch (ArgumentException)
		{
		}
	}

	private void Redirected(DataReceivedEventArgs e, ShellResponse shellResponse)
	{
		responseCount++;
		if (autoCloseWhenGoOnResponse && logMonitorType > 0 && responseTriggerCount < responseCount)
		{
			if (autoLockHandler != null && !autoLockHandler.SafeWaitHandle.IsClosed)
			{
				autoLockHandler.Set();
				autoLockHandler = null;
			}
			base.Recipe.UcDevice.MessageBox.Close(true);
		}
		if (string.IsNullOrEmpty(e.Data) || quit)
		{
			return;
		}
		string text = e.Data.Trim();
		string responseKey;
		ShellResponse.ShellCmdStatus shellCmdStatus = shellResponse.ParseResponse(text, out responseKey);
		base.Log.AddLog($"status: {shellCmdStatus}, shell response: {text}", upload: true);
		if (QuitConditionCheck(text))
		{
			if (text.Contains("No such file or directory"))
			{
				ToolErrorStatus = ShellResponse.ShellCmdStatus.FileLostError;
			}
			else if (text.Contains("STATUS_SEC_VIOLATE_ANTI_ROLLBACK"))
			{
				ToolErrorStatus = ShellResponse.ShellCmdStatus.FastbootDegrade;
			}
			else
			{
				ToolErrorStatus = ShellResponse.ShellCmdStatus.ConditionQuit;
			}
			quit = true;
			return;
		}
		ShowMessageAnalyzeResponse(text, shellResponse);
		AnalyzeComport(text);
		switch (shellCmdStatus)
		{
		case ShellResponse.ShellCmdStatus.Error:
		case ShellResponse.ShellCmdStatus.RomUnMatchError:
		case ShellResponse.ShellCmdStatus.AuthorizedError:
		case ShellResponse.ShellCmdStatus.FastbootError:
			if (!mError && string.IsNullOrEmpty(failedResponse))
			{
				failedResponse = text;
			}
			if (ToolErrorStatus == ShellResponse.ShellCmdStatus.None)
			{
				if (shellCmdStatus == ShellResponse.ShellCmdStatus.AuthorizedError)
				{
					IsConnected = true;
				}
				ToolErrorStatus = shellCmdStatus;
			}
			mError = true;
			break;
		case ShellResponse.ShellCmdStatus.Connected:
			if (!IsConnected)
			{
				base.Recipe.UcDevice.MessageBox.Close(true);
				base.Recipe.FreeEventHandler(realFlash: true);
				ShowMessageWhenConnectedSuccessAsync();
			}
			IsConnected = true;
			break;
		case ShellResponse.ShellCmdStatus.Downloading:
		{
			double downloadProgressPercent = shellResponse.GetDownloadProgressPercent(text, responseKey);
			ProgressUpdate(downloadProgressPercent);
			break;
		}
		case ShellResponse.ShellCmdStatus.Completed:
			mCompleted = true;
			break;
		case ShellResponse.ShellCmdStatus.None:
		case ShellResponse.ShellCmdStatus.Connecting:
		case ShellResponse.ShellCmdStatus.Outputing:
		case ShellResponse.ShellCmdStatus.Authenticating:
		case ShellResponse.ShellCmdStatus.Writing:
		case ShellResponse.ShellCmdStatus.FileLostError:
		case ShellResponse.ShellCmdStatus.FastbootDegrade:
		case ShellResponse.ShellCmdStatus.ConditionQuit:
			break;
		}
	}

	private async Task ShowMessageWhenStartToolAsync(ShellResponse shellResponse)
	{
		bool flag = true;
		if ((shellResponse.ShellCmd == ShellResponse.ShellCmdType.MTekFlashTool || shellResponse.ShellCmd == ShellResponse.ShellCmdType.MTekSpFlashTool) && base.Cache.ContainsKey("Read Device Mode") && base.Recipe.Device != null)
		{
			flag = base.Recipe.Device.ConnectType != ConnectType.Adb;
		}
		if (flag && base.Info.Args.ConnectTutorials is JObject jObject && ((IsRetry && jObject.Value<JArray>("ReSteps") != null) || jObject.Value<JArray>("Steps") != null))
		{
			string title = jObject.Value<string>("Title");
			JArray steps = jObject.Value<JArray>((IsRetry && jObject.Value<JArray>("ReSteps") != null) ? "ReSteps" : "Steps");
			bool autoPlay = jObject.Value<bool>("AutoPlay");
			double interval = jObject.Value<double>("Interval");
			LogHelper.LogInstance.Debug("Args.ConnectTutorials.Steps will show");
			if (!base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(title, steps, -1, autoPlay, interval).HasValue)
			{
				quit = true;
			}
		}
		else if (flag && base.Info.Args.ConnectSteps != null)
		{
			ConnectStepInfo _connectStep = new ConnectStepInfo();
			_connectStep.NoteText = base.Info.Args.ConnectSteps.NoteText?.ToString();
			_connectStep.RetryText = base.Info.Args.ConnectSteps.RetryText?.ToString();
			_connectStep.WidthRatio = base.Info.Args.ConnectSteps.WidthRatio?.ToString() ?? "1:2:2";
			await Task.Run(delegate
			{
				List<ConnectSteps> list = new List<ConnectSteps>();
				foreach (dynamic item in base.Info.Args.ConnectSteps.Steps)
				{
					list.Add(new ConnectSteps
					{
						StepImage = item.Image.ToString(),
						StepContent = item.Content.ToString()
					});
				}
				_connectStep.Steps = list;
				if (!base.Recipe.UcDevice.MessageBox.AutoCloseMoreStep(base.Name, _connectStep, -1, popupWhenClose: true).HasValue)
				{
					quit = true;
				}
			});
		}
		else
		{
			if (!((flag && base.Info.Args.PromptText != null) ? true : false))
			{
				return;
			}
			await Task.Run(delegate
			{
				dynamic val = base.Info.Args.Image?.ToString();
				dynamic val2 = base.Info.Args.PromptText.ToString();
				if (base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, val2, val, showClose: true, popupWhenClose: true) == null)
				{
					quit = true;
				}
			});
		}
	}

	private void ShowMessageWhenConnectedSuccessAsync()
	{
		Task.Run(delegate
		{
			if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("ReleaseSteps") != null)
			{
				JArray steps = jObject.Value<JArray>("ReleaseSteps");
				LogHelper.LogInstance.Debug("Args.ConnectTutorials.ReleaseSteps will show");
				base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(null, steps, 10000, autoPlay: false, 5000.0, showPlayControl: false, showClose: false);
			}
			else if (base.Info.Args.ExPromptText != null)
			{
				string image = base.Info.Args.ExImage?.ToString();
				string message = base.Info.Args.ExPromptText.ToString();
				base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, message, image, null, 10000, null, showClose: false, popupWhenClose: false, format: true, true);
			}
		});
	}

	private void ShowMessageAnalyzeResponse(string response, ShellResponse shellResponse)
	{
		if (base.Info.Args.LogMonitorActions != null)
		{
			foreach (dynamic iter in base.Info.Args.LogMonitorActions)
			{
				if (!(response.Contains(iter.MatchText.ToString()) ? true : false))
				{
					continue;
				}
				if (iter.ActionType == "Command")
				{
					string text = iter.EXE;
					if (text.StartsWith("$"))
					{
						text = base.Cache[text.TrimStart('$')];
					}
					string arg = ProcessRunner.ProcessString(text, iter.Command.ToString(), 6000);
					base.Log.AddLog($"Excute shell command: {text} {(object)iter.Command}, response: {arg}!", upload: true);
				}
				else
				{
					if (!((iter.Steps != null && iter.ActionType == "PromptText") ? true : false))
					{
						continue;
					}
					responseTriggerCount = responseCount;
					int millisecondsDelay = iter.Delay ?? ((object)0);
					autoCloseWhenGoOnResponse = iter.AutoCloseWhenGoOnResponse ?? ((object)false);
					if (iter.Steps.Count == 1)
					{
						dynamic img = iter.Steps[0].Image?.ToString();
						dynamic content = iter.Steps[0].Content.ToString();
						Task.Delay(millisecondsDelay).ContinueWith(delegate
						{
							if (!autoCloseWhenGoOnResponse || responseTriggerCount == responseCount)
							{
								Task.Run(delegate
								{
									autoLockHandler = new AutoResetEvent(initialState: false);
									logMonitorType = 1;
									int num = (int)((double)base.TimeoutMilliseconds - DateTime.Now.Subtract(startTime).TotalMilliseconds);
									bool? flag = base.Recipe.UcDevice.MessageBox.AutoClose(iter.NoteText?.ToString() ?? base.Name, content, img, new List<string> { "K0327" }, num, showClose: false, popupWhenClose: true, format: false, autoCloseResult: false);
									logMonitorType = 0;
									if (!flag.HasValue)
									{
										quit = true;
									}
									else if (flag == false)
									{
										mError = true;
									}
									autoLockHandler?.Set();
								});
							}
						});
					}
					else
					{
						if (!((iter.Steps.Count == 2 || iter.Steps.Count == 3) ? true : false))
						{
							continue;
						}
						ConnectStepInfo multiInfo = new ConnectStepInfo();
						multiInfo.NoteText = iter.NoteText?.ToString();
						multiInfo.WidthRatio = iter.WidthRatio?.ToString() ?? "1:2:2";
						multiInfo.Steps = new List<ConnectSteps>();
						foreach (dynamic item in iter.Steps)
						{
							multiInfo.Steps.Add(new ConnectSteps
							{
								StepImage = item.Image?.ToString(),
								StepContent = item.Content.ToString()
							});
						}
						Task.Delay(millisecondsDelay).ContinueWith(delegate
						{
							if (!autoCloseWhenGoOnResponse || responseTriggerCount == responseCount)
							{
								Task.Run(delegate
								{
									logMonitorType = 2;
									base.Recipe.UcDevice.MessageBox.AutoCloseMoreStep(base.Name, multiInfo);
									logMonitorType = 0;
								});
							}
						});
					}
				}
			}
		}
		if (response.Contains("too many links"))
		{
			base.Recipe.UcDevice.MessageBox.Show(base.Name, "K1452", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait(60000);
		}
		else if (!IsConnected && (response.StartsWith("Scanning USB port", StringComparison.CurrentCultureIgnoreCase) || response.StartsWith("Detecting download device", StringComparison.CurrentCultureIgnoreCase) || response.StartsWith("scan device START", StringComparison.CurrentCultureIgnoreCase)) && ConnectTutorialsTask == null)
		{
			base.Log.AddLog("the response meet the connection pop-up conditions, will show connection popup window");
			ConnectTutorialsTask = Task.Run(() => ShowMessageWhenStartToolAsync(shellResponse));
		}
	}

	protected void AnalyzeComport(string data)
	{
		if (!string.IsNullOrEmpty(data) && !base.Cache.ContainsKey("comport"))
		{
			string value = Regex.Match(data, "\\(COM(?<value>\\d+)\\)").Groups["value"].Value;
			if (!string.IsNullOrEmpty(value) && !base.Cache.ContainsKey("comport"))
			{
				base.Cache.Add("comport", value);
			}
		}
	}

	private bool CheckComport()
	{
		if (!base.Cache.ContainsKey("comport"))
		{
			return true;
		}
		List<string> comInfo = GlobalFun.GetComInfo();
		if (comInfo == null || comInfo.Count == 0)
		{
			return false;
		}
		return comInfo.Exists((string n) => n.Contains(string.Format("(COM{0})", (object)base.Cache["comport"])));
	}

	private bool QuitConditionCheck(string response)
	{
		foreach (JObject quitCondition in QuitConditionList)
		{
			List<string> list = quitCondition.Value<JArray>("Condition").Values<string>().ToList();
			if (list != null && list.Exists((string n) => Regex.IsMatch(response, n, RegexOptions.IgnoreCase)))
			{
				QuitConditionMessage = quitCondition.Value<string>("Message");
				string value = quitCondition.Value<string>("RescueMark");
				base.Log.AddInfo("rescuemark", value);
				base.Log.AddLog("shell response contains: " + string.Join(" | ", list), upload: true);
				return true;
			}
		}
		return false;
	}

	protected Task<List<string>> StartDeviceMonitorTask()
	{
		return Task.Run(delegate
		{
			List<string> result = new List<string>();
			List<string> cacheIds = new List<string>();
			deviceMonitorRunning = true;
			int num = 0;
			do
			{
				List<string> list = new List<string>();
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_PnPEntity"))
				{
					using ManagementObjectSearcher managementObjectSearcher2 = new ManagementObjectSearcher("Select * From Win32_USBController");
					List<ManagementBaseObject> list2 = new List<ManagementBaseObject>();
					foreach (ManagementBaseObject item in managementObjectSearcher.Get())
					{
						list2.Add(item);
					}
					foreach (ManagementBaseObject item2 in managementObjectSearcher2.Get())
					{
						list2.Add(item2);
					}
					foreach (ManagementBaseObject item3 in list2)
					{
						try
						{
							string text = item3.GetPropertyValue("DeviceID") as string;
							if (!string.IsNullOrEmpty(text))
							{
								string text2 = item3.GetPropertyValue("Name") as string;
								string text3 = item3.GetPropertyValue("Manufacturer") as string;
								string text4 = item3.GetPropertyValue("Status") as string;
								list.Add(text);
								if (num == 0 && !cacheIds.Contains(text))
								{
									cacheIds.Add(text);
								}
								else if (num > 0 && !cacheIds.Contains(text))
								{
									cacheIds.Add(text);
									string text5 = "Name: " + text2 + ", DeviceId: " + text + ", Manufacturer: " + text3 + ", Status: " + text4;
									if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text4) && text4.Equals("ok", StringComparison.CurrentCultureIgnoreCase))
									{
										using ManagementObjectSearcher managementObjectSearcher3 = new ManagementObjectSearcher("Select * From Win32_PnPSignedDriver");
										foreach (ManagementObject item4 in managementObjectSearcher3.Get())
										{
											if (item4.GetPropertyValue("DeviceID") as string == text)
											{
												string text6 = item4.GetPropertyValue("DriverProviderName") as string;
												string text7 = item4.GetPropertyValue("DriverVersion") as string;
												string text8 = item4.GetPropertyValue("DriverDate") as string;
												string text9 = item4.GetPropertyValue("Signer") as string;
												text5 = text5 + ", DriverProviderName: " + text6 + ", DriverVersion: " + text7 + ", DriverDate: " + text8 + ", Signer: " + text9;
												break;
											}
										}
									}
									text5 = $"device connected-{DateTime.Now:HH:mm:ss}: {text5}";
									result.Add(text5);
								}
							}
						}
						catch
						{
						}
					}
					cacheIds.Except(list).ToList().ForEach(delegate(string n)
					{
						result.Add($"device removed-{DateTime.Now:HH:mm:ss}: {n}");
						cacheIds.Remove(n);
					});
				}
				num++;
			}
			while (deviceMonitorRunning);
			if (result.Count == 0)
			{
				result.Add("no device connected");
			}
			return result;
		});
	}

	protected void FreeLock()
	{
		if (autoLockHandler != null)
		{
			autoLockHandler.WaitOne();
			autoLockHandler.Dispose();
			autoLockHandler = null;
		}
	}
}
