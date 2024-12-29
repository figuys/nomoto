using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class CmdRunner : BaseStep
{
	private List<string> decryptFiles;

	private Action closeResuingWndAction;

	private bool quit;

	private Action closeMessageboxAction;

	private volatile bool IsConnected;

	private volatile bool IsCompleted;

	private volatile bool IsError;

	private double totalFile;

	private double currentFile;

	private static Dictionary<string, ShellResponse.ShellCmdStatus> ResponseToStatus = new Dictionary<string, ShellResponse.ShellCmdStatus>
	{
		{
			"Device successfully AVB unlocked",
			ShellResponse.ShellCmdStatus.Connected
		},
		{
			"Finished. Total time",
			ShellResponse.ShellCmdStatus.Downloading
		},
		{
			"Press any key to reboot",
			ShellResponse.ShellCmdStatus.Completed
		},
		{
			"flash successed",
			ShellResponse.ShellCmdStatus.Completed
		},
		{
			"fastboot: error",
			ShellResponse.ShellCmdStatus.Error
		}
	};

	public override void Run()
	{
		string text = base.Info.Args.Command;
		if (base.Info.Args.Format != null)
		{
			List<object> list = new List<object>();
			foreach (object item2 in base.Info.Args.Format)
			{
				string text2 = (string)(dynamic)item2;
				object item = text2;
				if (text2.StartsWith("$"))
				{
					string key = text2.Substring(1);
					item = base.Cache[key];
				}
				list.Add(item);
			}
			text = string.Format(text, list.ToArray());
		}
		base.Log.AddLog("CmdRunner execute cmd: " + text);
		MatchCollection matchCollection = Regex.Matches(File.ReadAllText(text), "^fastboot\\s+?[^\\\"]+\\\".+\\\"\\s*$", RegexOptions.Multiline);
		totalFile = matchCollection.Count;
		if (base.Info.Args.PromptText != null)
		{
			Task.Run(delegate
			{
				dynamic val = base.Info.Args.Image?.ToString();
				dynamic val2 = base.Info.Args.PromptText.ToString();
				dynamic val3 = base.Info.Args.link?.ToString();
				if (base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, val2, val, link: val3, milliseconds: 10000) == null)
				{
					quit = true;
				}
			});
		}
		try
		{
			string text3 = base.Info.Args.DecryptFileType;
			if (!string.IsNullOrEmpty(text3))
			{
				decryptFiles = GlobalFun.DecryptRomFile(base.Resources.Get("Rom"), text3);
			}
		}
		catch (Exception)
		{
		}
		GlobalFun.KillProcess("cmd");
		GlobalFun.KillProcess("fastboot");
		Thread.Sleep(3000);
		IsConnected = false;
		string workingDirectory = base.Resources.Get(RecipeResources.Rom);
		Process process = new Process();
		process.StartInfo.FileName = "cmd.exe";
		process.StartInfo.WorkingDirectory = workingDirectory;
		process.StartInfo.Arguments = "";
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.Verb = "runas";
		process.EnableRaisingEvents = true;
		process.StartInfo.CreateNoWindow = true;
		List<string> outputList = new List<string>();
		process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
		{
			Redirected(outputList, sender, e);
		};
		process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
		{
			Redirected(outputList, sender, e);
		};
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		process.StandardInput.WriteLine("\"" + text + "\" &exit");
		int num = 0;
		int num2 = ((base.Info.Args.Timeout == null) ? ((object)1800000) : base.Info.Args.Timeout);
		bool flag = false;
		do
		{
			flag = process.HasExited;
			if (IsCompleted || IsError || flag || quit || (num >= 600000 && !IsConnected))
			{
				break;
			}
			Thread.Sleep(1000);
			num += 1000;
		}
		while (num < num2);
		if (!flag)
		{
			process.Kill();
		}
		GlobalFun.KillProcess("cmd");
		GlobalFun.KillProcess("fastboot");
		Thread.Sleep(3000);
		Result result = Result.PASSED;
		string response = null;
		if (quit)
		{
			result = Result.MANUAL_QUIT;
			response = "quit";
		}
		else if (!IsConnected)
		{
			result = Result.DEVICE_CONNECT_FAILED;
			response = "device connected timeout";
		}
		else if (!IsCompleted)
		{
			result = Result.FAILED;
			response = string.Join("\r\n", outputList);
		}
		closeMessageboxAction?.Invoke();
		closeMessageboxAction = null;
		closeResuingWndAction?.Invoke();
		closeResuingWndAction = null;
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
		base.Log.AddResult(this, result, response);
	}

	private void Redirected(List<string> outputList, object sender, DataReceivedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.Data))
		{
			return;
		}
		outputList.Add(e.Data);
		string text = null;
		base.Log.AddLog("flash cmd output: " + e.Data);
		foreach (string key in ResponseToStatus.Keys)
		{
			if (Regex.IsMatch(e.Data, key, RegexOptions.IgnoreCase))
			{
				text = key;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		ShellResponse.ShellCmdStatus value = ShellResponse.ShellCmdStatus.None;
		if (!ResponseToStatus.TryGetValue(text, out value))
		{
			return;
		}
		base.Log.AddLog($"====>>Cmd runner response status:{value}!");
		switch (value)
		{
		case ShellResponse.ShellCmdStatus.Connected:
			base.Recipe.FreeEventHandler(realFlash: true);
			IsConnected = true;
			closeMessageboxAction?.Invoke();
			closeMessageboxAction = null;
			break;
		case ShellResponse.ShellCmdStatus.Downloading:
		{
			if (!IsConnected)
			{
				IsConnected = true;
				closeMessageboxAction?.Invoke();
				closeMessageboxAction = null;
			}
			if (!(totalFile > 0.0))
			{
				break;
			}
			currentFile += 1.0;
			double result = currentFile / totalFile;
			if (double.TryParse($"{result * 100.0:0.00}", out result))
			{
				double num = ((100.0 - result < 0.02) ? 100.0 : result);
				if (num > 0.0)
				{
					ProgressUpdate(num);
				}
			}
			break;
		}
		case ShellResponse.ShellCmdStatus.Completed:
			IsCompleted = true;
			break;
		case ShellResponse.ShellCmdStatus.Error:
			IsError = true;
			break;
		case ShellResponse.ShellCmdStatus.None:
		case ShellResponse.ShellCmdStatus.Connecting:
			break;
		}
	}
}
