using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootFlashSinglepartition : BaseStep
{
	private string failedResponse;

	private static readonly Regex REG_EX = new Regex("(?<bootloader>\\(bootloader\\)\\s+)(?<key>.+):\\s+(?<value>.*)");

	private List<KeyValuePair<string, int>> commands;

	public override void Run()
	{
		string text = base.Info.Args.XML;
		string text2 = base.Info.Args.EXE;
		string name = base.Info.Args.PartitionName;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (!File.Exists(text))
		{
			if (Directory.Exists(text))
			{
				base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "Rom resource file: " + text + " not exist!");
			}
			else
			{
				base.Log.AddResult(this, Result.ABORTED, "Mybe the key \"XMl\" in json args is error!");
			}
			return;
		}
		bool flag = ExtractBootloaderCommand(text, name);
		if (!flag)
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "Rom resource file: " + text + " not exist!");
			return;
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "fastboot.exe";
		}
		string text3 = base.Resources.GetLocalFilePath(text2);
		if (string.IsNullOrEmpty(text3))
		{
			text3 = Configurations.FastbootPath;
		}
		foreach (KeyValuePair<string, int> command in commands)
		{
			string text4 = EncapsulationFastbootCommand(command.Key);
			string text5 = ProcessRunner.ProcessString(text3, text4, command.Value)?.ToLower();
			base.Log.AddLog("fastboot command: " + text4 + ", response: " + text5, upload: true);
			if (text5.Contains("fail") || text5.Contains("error"))
			{
				failedResponse = "exec command failed: " + text4;
				flag = false;
				break;
			}
		}
		dynamic val = base.Info.Args.IgnoreCurrStepResult ?? ((object)false);
		if (flag)
		{
			base.Log.AddResult(this, Result.PASSED);
			return;
		}
		if ((!val))
		{
			string text6 = string.Empty;
			List<string> list = ProcessRunner.ProcessList(text3, EncapsulationFastbootCommand("getvar all"), 12000);
			if (list != null)
			{
				foreach (string item in list)
				{
					Match match = REG_EX.Match(item);
					string value = match.Groups["key"].Value;
					string value2 = match.Groups["value"].Value;
					if (!string.IsNullOrEmpty(value) && value.Equals("channelid"))
					{
						base.Log.AddInfo("channelid", value2);
						break;
					}
				}
				text6 = string.Join("\r\n", list);
			}
			base.Log.AddLog("fastboot command : getvar all, response: " + text6, upload: true);
			text6 = ProcessRunner.ProcessString(text3, EncapsulationFastbootCommand("oem read_sv"), 12000)?.ToLower();
			base.Log.AddLog("fastboot command : oem read_sv, response: " + text6, upload: true);
			text6 = ProcessRunner.ProcessString(text3, EncapsulationFastbootCommand("oem partition"), 12000)?.ToLower();
			base.Log.AddLog("fastboot command : oem partition, response: " + text6, upload: true);
		}
		base.Log.AddResult(this, Result.FASTBOOT_FLASH_SINGLEPARTITION_FAILED, failedResponse);
	}

	private bool ExtractBootloaderCommand(string xml, string name)
	{
		string text = string.Empty;
		try
		{
			text = File.ReadAllText(xml);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(text);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes($"//step[@partition='{name}']");
			if (xmlNodeList != null && xmlNodeList.Count == 0)
			{
				failedResponse = "partition=" + name + " not exists in file: " + xml;
				base.Log.AddLog(failedResponse, upload: true);
				return false;
			}
			string directoryName = Path.GetDirectoryName(xml);
			commands = new List<KeyValuePair<string, int>>();
			foreach (XmlNode item in xmlNodeList)
			{
				XmlNode namedItem;
				string text2 = (((namedItem = item.Attributes.GetNamedItem("operation")) != null) ? namedItem.Value : string.Empty).Trim();
				string text3 = (((namedItem = item.Attributes.GetNamedItem("partition")) != null) ? namedItem.Value : string.Empty).Trim();
				string path = (((namedItem = item.Attributes.GetNamedItem("filename")) != null) ? namedItem.Value : string.Empty).Trim();
				(((namedItem = item.Attributes.GetNamedItem("MD5")) != null) ? namedItem.Value : string.Empty).Trim();
				string text4 = Path.Combine(directoryName, path);
				string key = text2 + " " + text3 + " \"" + text4 + "\"";
				commands.Add(new KeyValuePair<string, int>(key, 300000));
			}
			return true;
		}
		catch (XmlException ex)
		{
			base.Log.AddLog("Error xml content: " + text, upload: false, ex);
			return false;
		}
		catch (Exception ex2)
		{
			base.Log.AddLog("Read xml for exucte bootloader command failed!", upload: false, ex2);
			return false;
		}
	}
}
