using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootModifyFlashFile : BaseStep
{
	public override void Run()
	{
		string text = base.Info.Args.DefaultSlot;
		if (string.IsNullOrEmpty(text))
		{
			base.Log.AddLog("recipe config defaultSlot is null");
			base.Log.AddResult(this, Result.FASTBOOT_SLOT_SET_FAILED, "recipe config error");
			return;
		}
		string text2 = base.Info.Args.Command?.ToString();
		string text3 = EncapsulationFastbootCommand(text2);
		if (string.IsNullOrEmpty(text3))
		{
			base.Log.AddLog("recipe config command is null");
			base.Log.AddResult(this, Result.FASTBOOT_SLOT_SET_FAILED, "recipe config error");
			return;
		}
		string text4 = ProcessRunner.ProcessString(Configurations.FastbootPath, text3, 6000)?.ToLower();
		base.Log.AddLog("fastboot command : " + text3 + ", response: " + text4, upload: true);
		string value = null;
		if (!string.IsNullOrEmpty(text4))
		{
			string[] array = text4.Split(new string[1] { "\r\n" }, StringSplitOptions.None);
			string value2 = text2.Substring(text2.IndexOf(' '))?.Trim();
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Contains(value2))
					{
						array = array[i].Split(':');
						if (array != null && array.Length > 1)
						{
							value = array[1]?.Trim();
						}
						break;
					}
				}
			}
		}
		if (string.IsNullOrEmpty(value))
		{
			base.Log.AddLog("current slot is null", upload: true);
			base.Log.AddResult(this, Result.FASTBOOT_SLOT_SET_FAILED, "current slot not found");
			return;
		}
		if (!text.Equals(value, StringComparison.CurrentCultureIgnoreCase))
		{
			base.Log.AddLog("current slot is not equal default solt");
			string key = base.Info.Args.File;
			string text5 = base.Cache[key];
			base.Log.AddLog("old flash file: " + text5);
			string pa = "(partition\\s*=\\s*\"[^\"]+?)_a\"";
			string pb = "(partition\\s*=\\s*\"[^\"]+?)_b\"";
			List<string> result = new List<string>();
			File.ReadAllLines(text5).ToList().ForEach(delegate(string n)
			{
				if (Regex.IsMatch(n, pa))
				{
					result.Add(Regex.Replace(n, pa, "$1_b\""));
				}
				else if (Regex.IsMatch(n, pb))
				{
					result.Add(Regex.Replace(n, pb, "$1_a\""));
				}
				else
				{
					result.Add(n);
				}
			});
			string text6 = Path.Combine(Directory.GetParent(text5).FullName, "flash.temp.xml");
			if (File.Exists(text6))
			{
				File.Delete(text6);
			}
			File.WriteAllLines(text6, result, Encoding.UTF8);
			base.Cache[key] = text6;
			base.Log.AddLog("new flash file: " + text6);
		}
		base.Log.AddResult(this, Result.PASSED);
	}
}
