using System;
using System.Management;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootShell : BaseStep
{
	public override void Run()
	{
		string text = string.Empty;
		Result result = Result.PASSED;
		if (base.Info.Args.Data != null)
		{
			text = base.Info.Args.Data;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
		}
		string text2 = (string.IsNullOrEmpty(text) ? base.Info.Args.Command : $"{(object)base.Info.Args.Command} {text}");
		if (string.IsNullOrEmpty(text2))
		{
			base.Log.AddResult(this, Result.FAILED, "Command is not set!");
			return;
		}
		text2 = EncapsulationFastbootCommand(text2);
		bool flag = false;
		string exe = LoadToolPath();
		string empty = string.Empty;
		_ = base.Recipe.Device?.Identifer;
		int timeout = base.Info.Args.Timeout ?? ((object)60000);
		do
		{
			empty = ProcessRunner.ProcessString(exe, text2, timeout)?.ToLower();
			base.Log.AddLog("fastboot command: " + text2 + ", response: " + empty, upload: true);
			flag = IsStrHasErrorMark(empty);
			if (flag && base.Info.Args.Print == true)
			{
				PrintDevStatus();
			}
		}
		while (flag && Retry-- > 0);
		string response = null;
		if (!string.IsNullOrEmpty(empty) && empty.Contains("STATUS_SEC_VIOLATE_ANTI_ROLLBACK"))
		{
			result = Result.FASTBOOT_DEGRADE_QUIT;
			response = "STATUS_SEC_VIOLATE_ANTI_ROLLBACK";
		}
		else if (flag)
		{
			result = Result.FASTBOOT_SHELL_FAILED;
			response = AnalysisFailedResponse(empty);
		}
		string[] array = empty?.Split(new string[1] { "\r\n" }, StringSplitOptions.None);
		if (array != null && array.Length != 0)
		{
			base.RunResult = array[0];
		}
		base.Log.AddResult(this, result, response);
	}

	private void PrintDevStatus()
	{
		ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher("Select * From Win32_PnPEntity where service='WinUSB' or Status='ERROR'").Get();
		string text = string.Empty;
		foreach (ManagementBaseObject item in managementObjectCollection)
		{
			string text2 = item.GetPropertyValue("Status") as string;
			string text3 = item.GetPropertyValue("Service") as string;
			if (!text2.Equals("ERROR", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(text3))
			{
				text += string.Format("Name: {0}\tService: {1}\tStatus: {2}\r\n", item.GetPropertyValue("Name"), text3, text2);
			}
		}
		base.Log.AddLog("::::::::::::::::::::::::::Detect device status:::::::::::::::::::::::::::::::::", upload: true);
		if (!string.IsNullOrEmpty(text))
		{
			base.Log.AddLog(text, upload: true);
		}
		base.Log.AddLog("Fastboot device isn't find!", upload: true);
		base.Log.AddLog(":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::", upload: true);
	}

	private bool IsStrHasErrorMark(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		if (!str.Contains("error"))
		{
			return str.Contains("fail");
		}
		return true;
	}
}
