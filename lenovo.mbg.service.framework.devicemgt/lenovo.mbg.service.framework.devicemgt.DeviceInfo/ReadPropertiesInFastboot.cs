using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class ReadPropertiesInFastboot
{
	private static readonly Regex REG_EX = new Regex("(?<bootloader>\\(bootloader\\)\\s+)(?<key>.+):\\s+(?<value>.*)");

	protected DeviceEx device;

	public Dictionary<string, string> Props { get; private set; }

	public ReadPropertiesInFastboot(DeviceEx device)
	{
		this.device = device;
		Props = new Dictionary<string, string>();
	}

	public void Run()
	{
		Dictionary<string, string> dictionary = ReadAll();
		if (dictionary.Count > 0)
		{
			Props.Clear();
			Props = new Dictionary<string, string>(dictionary);
			ReadSimConfig();
		}
		ConvertFsgVersion();
		ConvertFingerPrint();
		ConvertBlurVersion();
		ConvertFlashSize();
		ConvertRamSize();
	}

	public string GetProp(string element)
	{
		Props.TryGetValue(element, out var value);
		return value;
	}

	private Dictionary<string, string> ReadAll()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		List<string> list = new List<string>();
		string text = "getvar all";
		if (!string.IsNullOrEmpty(device.Identifer))
		{
			text = "-s " + device.Identifer + " " + text;
		}
		for (int i = 0; i < 4; i++)
		{
			list = ProcessRunner.ProcessList(Configurations.FastbootPath, text, 5000);
			if (list.Count > 0 && list.Count((string n) => n.Contains("execute error, commnad timeout")) == 0)
			{
				LogHelper.LogInstance.Info("Fastboot device exucte " + text + " successed!");
				break;
			}
		}
		foreach (string item in list)
		{
			Match match = REG_EX.Match(item);
			string value = match.Groups["key"].Value;
			string value2 = match.Groups["value"].Value;
			if (!string.IsNullOrEmpty(value) && !dictionary.ContainsKey(value))
			{
				dictionary.Add(value, value2?.Trim());
			}
		}
		return dictionary;
	}

	private void ConvertFsgVersion()
	{
		string text = Convert("version-baseband");
		if (!string.IsNullOrEmpty(text) && !text.Contains("not found"))
		{
			string[] array = text.Split(' ');
			if (array.Length == 1)
			{
				text = array[0].Trim();
			}
			else if (array.Length > 1)
			{
				text = array[1].Trim();
			}
		}
		Props["version-baseband"] = text;
	}

	private void ConvertFingerPrint()
	{
		string text = Convert("ro.build.fingerprint");
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split('/');
			if (array.Length > 3)
			{
				Props["softwareVersion"] = array[3].Trim();
			}
			if (array.Length > 2)
			{
				string[] array2 = array[2].Split(':');
				if (array2.Length > 1)
				{
					Props["androidVer"] = array2[1]?.Trim();
				}
			}
		}
		Props["ro.build.fingerprint"] = text;
	}

	private void ConvertBlurVersion()
	{
		string value = Convert("ro.build.version.full");
		Props["ro.build.version.full"] = value;
	}

	private void ConvertFlashSize()
	{
		string text = Convert("emmc");
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(' ');
			Props["emmc"] = array[0];
		}
	}

	private void ConvertRamSize()
	{
		string text = Convert("ram");
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(' ');
			Props["ram"] = array[0];
		}
	}

	private void ReadSimConfig()
	{
		string text = "oem hw dualsim";
		if (!string.IsNullOrEmpty(device.Identifer))
		{
			text = "-s " + device.Identifer + " " + text;
		}
		List<string> list = ProcessRunner.ProcessList(Configurations.FastbootPath, text, 20000);
		string text2 = null;
		foreach (string item in list)
		{
			if (item.Contains("dualsim"))
			{
				string[] array = item.Split(':');
				if (array.Length > 1 && array[1].Trim() == "true")
				{
					text2 = "Dual";
				}
			}
		}
		if (text2 == null)
		{
			text2 = GetProp("dualsim");
			text2 = ((string.IsNullOrEmpty(text2) || !(text2.ToLower() == "true")) ? "Single" : "Dual");
		}
		Props["oem hw dualsim"] = text2;
	}

	private string Convert(string element)
	{
		if (!Props.TryGetValue(element, out var value))
		{
			int num = 0;
			value = string.Empty;
			bool flag;
			do
			{
				flag = Props.TryGetValue($"{element}[{num}]", out var value2);
				if (flag)
				{
					value += value2;
					num++;
				}
				else
				{
					value = value.Trim();
				}
			}
			while (flag);
		}
		return value;
	}
}
