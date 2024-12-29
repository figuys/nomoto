using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class BatFileVersionCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = base.Info.Args.EXE;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		int timeOut = ((base.Info.Args.Timeout == null) ? ((object)6000) : base.Info.Args.Timeout);
		string text2 = base.Info.Args.Command;
		List<object> list = new List<object>();
		if (base.Info.Args.Format != null)
		{
			foreach (object item2 in base.Info.Args.Format)
			{
				string text3 = (string)(dynamic)item2;
				object item = text3;
				if (text3.StartsWith("$"))
				{
					string key2 = text3.Substring(1);
					item = base.Cache[key2];
				}
				list.Add(item);
			}
			text2 = string.Format(text2, list.ToArray());
		}
		string empty = string.Empty;
		do
		{
			empty = ProcessRunner.CmdExcuteWithExit(text + " " + text2, base.Resources.Get("Rom"), timeOut);
			if (string.IsNullOrEmpty(empty))
			{
				continue;
			}
			int result = int.MinValue;
			int result2 = int.MinValue;
			string[] array = empty.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			_ = string.Empty;
			string[] array2 = array;
			foreach (string text4 in array2)
			{
				if (text4.Contains("device rollback id sum:"))
				{
					string[] array3 = text4.Trim().Split(':');
					if (array3.Length == 2 && !int.TryParse(array3[1], out result2))
					{
						result2 = int.MinValue;
					}
				}
				if (text4.Contains("signinfo id sum:"))
				{
					string[] array4 = text4.Trim().Split(':');
					if (array4.Length == 2 && !int.TryParse(array4[1], out result))
					{
						result = int.MinValue;
					}
				}
			}
			if (result2 != int.MinValue && result != int.MinValue)
			{
				if (result >= result2)
				{
					base.Log.AddResult(this, Result.PASSED, "bat check rom upgrade!");
					return;
				}
				base.Recipe.UcDevice.MessageBox.Show("K0711", "K1119").Wait();
				base.Log.AddResult(this, Result.FASTBOOT_DEGRADE_QUIT, "bat check rom upgrade!");
				return;
			}
		}
		while (Retry-- > 0);
		base.Log.AddLog($"Bat file {(object)base.Info.Args.EXE} exucte failed! Can't get device rollbackid or rom version.");
		base.Log.AddResult(this, Result.FAILED, "bat file excute failed!");
	}
}
