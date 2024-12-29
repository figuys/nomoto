using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class RunningTimeCheck : BaseStep
{
	private enum INSTALLSTATE
	{
		INSTALLSTATE_ABSENT = 2,
		INSTALLSTATE_ADVERTISED = 1,
		INSTALLSTATE_DEFAULT = 5,
		INSTALLSTATE_INVALIDARG = -2,
		INSTALLSTATE_UNKNOWN = -1
	}

	public static List<string> VcGuidList = new List<string> { "{F0C3E5D1-1ADE-321E-8167-68EF0DE699A5}" };

	public override void Run()
	{
		Result result = Result.PASSED;
		if (base.Cache.ContainsKey("recoveryExe"))
		{
			string fileName = base.Cache["recoveryExe"];
			string path = "BMAFrame9.dll";
			string text = Path.Combine(new FileInfo(fileName).Directory.FullName, path);
			if (!File.Exists(text))
			{
				File.Copy(Path.Combine(Environment.CurrentDirectory, path), text, overwrite: true);
			}
		}
		if (!CheckVcRunnigEnv())
		{
			string message = base.Info.Args.PromptText.ToString();
			string image = base.Info.Args.Image?.ToString();
			List<string> list = new List<string>();
			string ok = null;
			string cancel = null;
			if (base.Info.Args.ButtonContent != null)
			{
				foreach (object item2 in base.Info.Args.ButtonContent)
				{
					string item = (string)(dynamic)item2;
					list.Add(item);
				}
				if (list.Count == 1)
				{
					ok = list[0];
				}
				else if (list.Count == 2)
				{
					ok = list[0];
					cancel = list[1];
				}
			}
			if (base.Recipe.UcDevice.MessageBox.RightPic(base.Info.Name, message, image, ok, cancel).Result == true)
			{
				ProcessRunner.ProcessString(Path.Combine(Environment.CurrentDirectory, "vcredist_x86_2010.exe"), null, -1);
				if (!CheckVcRunnigEnv())
				{
					result = Result.INSTALL_VC_RUNNINGTIME_FAILED;
				}
			}
			else
			{
				result = Result.QUIT;
			}
		}
		base.Log.AddResult(this, result, (result == Result.PASSED) ? null : "manual quit");
	}

	private bool CheckVcRunnigEnv()
	{
		foreach (string vcGuid in VcGuidList)
		{
			if (MsiQueryProductState(vcGuid).Equals(INSTALLSTATE.INSTALLSTATE_DEFAULT))
			{
				return true;
			}
		}
		base.Log.AddLog("VC++ running time not exists");
		return false;
	}

	[DllImport("msi.dll")]
	private static extern INSTALLSTATE MsiQueryProductState(string product);
}
