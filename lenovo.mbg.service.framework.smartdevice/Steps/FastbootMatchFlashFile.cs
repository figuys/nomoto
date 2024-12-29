using System;
using System.Linq;
using lenovo.mbg.service.common.utilities;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootMatchFlashFile : BaseStep
{
	public override void Run()
	{
		Result result = Result.FASTBOOT_FLASH_FILE_MATCH_FAILED;
		string text = EncapsulationFastbootCommand(base.Info.Args.Command?.ToString());
		string text2 = ProcessRunner.ProcessString(Configurations.FastbootPath, text, 6000)?.ToLower();
		base.Log.AddLog("fastboot command : " + text + ", response: " + text2, upload: true);
		string cond = null;
		if (!string.IsNullOrEmpty(text2))
		{
			string[] array = text2.Split(new string[1] { "\r\n" }, StringSplitOptions.None);
			string value = text.Substring(text.IndexOf(' '))?.Trim();
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Contains(value))
					{
						array = array[i].Split(':');
						if (array != null && array.Length > 1)
						{
							cond = array[1]?.Trim();
						}
						break;
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(cond))
		{
			string text3 = ((JArray)JArray.Parse(base.Info.Args.CondMap.ToString())).FirstOrDefault((JToken n) => n.Value<string>("cond") == cond).Value<string>("value");
			base.Cache.Add("FastbootMatchFlashFile", text3);
			base.Log.AddLog("match filename: " + text3, upload: true);
			result = Result.PASSED;
		}
		base.Log.AddResult(this, result, (result == Result.PASSED) ? null : "fastboot match flash file failed");
	}
}
