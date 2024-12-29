using System.Collections.Generic;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class CheckFingerPrintVersion : ICheckVersion
{
	public bool Check(string device, string config, Dictionary<string, string> aparams)
	{
		bool num = Process(device, config);
		if (!num && aparams != null)
		{
			CollectionDegradeInfo(aparams);
		}
		return num;
	}

	public void CollectionDegradeInfo(Dictionary<string, string> aparams)
	{
		FlashContext.SingleInstance.service.RequestContent(WebServicesContext.COLLECTION_DOWNGRADE_INFO, aparams);
	}

	private bool Process(string device, string config)
	{
		if (string.IsNullOrEmpty(device) || string.IsNullOrEmpty(config))
		{
			return true;
		}
		string text = ParseVersion(device);
		string text2 = ParseVersion(config);
		if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
		{
			return true;
		}
		LogHelper.LogInstance.Info("check fingerprint version, device version: " + device + ", config version: " + config, upload: true);
		char c = text.ToUpperInvariant()[0];
		char c2 = text2.ToUpperInvariant()[0];
		if (c2 != c)
		{
			if (c2 < c)
			{
				LogHelper.LogInstance.Debug($"check fingerprint version, device version '{c}' is downgrade from config version '{c2}'", upload: true);
				return false;
			}
			return true;
		}
		string alphas;
		List<int> device2 = PullVersionNumbers(text, out alphas);
		string alphas2;
		List<int> config2 = PullVersionNumbers(text2, out alphas2);
		if (alphas.Length < 3 || alphas2.Length < 3 || alphas.Substring(1, 2).ToLowerInvariant() != alphas2.Substring(1, 2).ToLowerInvariant())
		{
			return true;
		}
		if (text.Replace(alphas, string.Empty) == text2.Replace(alphas2, string.Empty))
		{
			if (text.Length > text2.Length)
			{
				LogHelper.LogInstance.Debug($"check fingerprint version, device version '{c}' is downgrade from config version '{c2}'", upload: true);
				return false;
			}
			return true;
		}
		if (IsDownGrade(device2, config2))
		{
			LogHelper.LogInstance.Debug("check fingerprint version, device version '" + device + "' is downgrade from config version '" + config + "'", upload: true);
			return false;
		}
		return true;
	}

	private string ParseVersion(string input)
	{
		string[] array = input.Split('/');
		if (array.Length > 3)
		{
			input = array[3].Trim();
		}
		Regex regex = new Regex("^[a-z]{3,4}[0-9]{2,3}\\.[a-z]?[0-9]+[a-z]?([-.][a-z]?[0-9]+)*", RegexOptions.IgnoreCase);
		string[] array2 = input.Split(' ');
		foreach (string text in array2)
		{
			if (regex.IsMatch(text))
			{
				return text;
			}
		}
		return null;
	}

	private List<int> PullVersionNumbers(string parsed, out string alphas)
	{
		List<char> list = new List<char>();
		list.Add('.');
		list.Add('-');
		alphas = string.Empty;
		string text = string.Empty;
		List<int> list2 = new List<int>();
		for (int i = 0; i < parsed.Length; i++)
		{
			char c = parsed[i];
			if (char.IsLetter(c))
			{
				alphas += c;
			}
			else if (char.IsDigit(c))
			{
				text += c;
			}
			else if (list.Contains(c) && text.Length > 0)
			{
				int item = int.Parse(text);
				list2.Add(item);
				text = string.Empty;
			}
		}
		if (text.Length > 0)
		{
			int item2 = int.Parse(text);
			list2.Add(item2);
			text = string.Empty;
		}
		alphas = alphas.Trim();
		return list2;
	}

	private bool IsDownGrade(List<int> device, List<int> config)
	{
		int count = device.Count;
		for (int i = 0; i < count; i++)
		{
			if (i >= config.Count)
			{
				return true;
			}
			int num = device[i];
			int num2 = config[i];
			if (num2 < num)
			{
				return true;
			}
			if (num2 > num)
			{
				return false;
			}
		}
		return false;
	}
}
