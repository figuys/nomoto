using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootFlash : BaseStep
{
	private static Dictionary<string, int> OperationToTimeout = new Dictionary<string, int>
	{
		{ "flash", 300000 },
		{ "erase", 60000 },
		{ "oem", 60000 },
		{ "getvar", 20000 },
		{ "reboot", 20000 },
		{ "reboot-bootloader", 10000 },
		{ "format", 60000 },
		{ "flashall", 600000 },
		{ "continue", 10000 }
	};

	private Action closeResuingWndAction;

	private List<CommandLine> commands;

	private string failedResponse;

	private int outCondition;

	private static readonly Regex REG_EX = new Regex("(?<bootloader>\\(bootloader\\)\\s+)(?<key>.+):\\s+(?<value>.*)");

	protected string fullpath;

	protected long totalDataSize;

	private string TAG => GetType().FullName;

	private string logPath => Configurations.RescueFailedLogPath;

	public override void Run()
	{
		outCondition = ParseCondition(base.OutCondition);
		string text = base.Info.Args.XML;
		_ = (string)base.Info.Args.EXE;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		fullpath = LoadToolPath();
		Result result = ParseCommands(text);
		if (File.Exists(logPath))
		{
			File.Delete(logPath);
		}
		string text2 = string.Empty;
		if (result == Result.PASSED && commands.Count > 0)
		{
			result = ExecuteCommand();
		}
		bool flag = false;
		if (base.Info.Args.IgnoreCurrStepResult != null)
		{
			flag = base.Info.Args.IgnoreCurrStepResult;
		}
		if (result != Result.PASSED && !flag)
		{
			List<string> list = ProcessRunner.ProcessList(fullpath, EncapsulationFastbootCommand("getvar all"), 12000);
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
				text2 = string.Join("\r\n", list);
			}
			base.Log.AddLog("command : getvar all, response: " + text2, upload: true);
			text2 = ProcessRunner.ProcessString(fullpath, EncapsulationFastbootCommand("oem read_sv"), 12000)?.ToLower();
			base.Log.AddLog("command : oem read_sv, response: " + text2, upload: true);
			text2 = ProcessRunner.ProcessString(fullpath, EncapsulationFastbootCommand("oem partition"), 12000)?.ToLower();
			base.Log.AddLog("command : oem partition, response: " + text2, upload: true);
		}
		if (result != Result.PASSED)
		{
			base.Recipe.PrintComInfo();
		}
		base.Log.AddResult(this, result, failedResponse);
	}

	private int ParseCondition(string condition)
	{
		DeviceEx device = base.Recipe.Device;
		if (string.IsNullOrEmpty(condition))
		{
			return 0;
		}
		if (device != null && device.Property != null)
		{
			int num = condition.LastIndexOf(':');
			string name = condition.Substring(0, num);
			string value = condition.Substring(num + 1);
			string propertyValue = device.Property.GetPropertyValue(name);
			if (!string.IsNullOrEmpty(propertyValue) && !string.IsNullOrEmpty(value) && propertyValue.Equals(value, StringComparison.CurrentCultureIgnoreCase))
			{
				return 2;
			}
		}
		return 1;
	}

	private Result ExecuteCommand()
	{
		Result result = Result.PASSED;
		int num = -1;
		if (!string.IsNullOrEmpty(base.condition))
		{
			CommandLine commandLine = commands.LastOrDefault((CommandLine n) => n.command.Contains(base.condition));
			if (commandLine.id > 0)
			{
				num = commandLine.id + 1;
			}
		}
		base.Recipe.FreeEventHandler(realFlash: true);
		bool flag = true;
		string empty = string.Empty;
		long num2 = 0L;
		_ = base.Recipe.Device?.Identifer;
		foreach (CommandLine command in commands)
		{
			if (base.ConditionSkipCommands != null)
			{
				JObject jObject = base.ConditionSkipCommands.FirstOrDefault((JObject n) => command.command.Contains(n.Value<string>("command")));
				if (jObject != null)
				{
					int num3 = ParseCondition(jObject.Value<string>("condition"));
					if (num3 == 2 || (num3 == 0 && outCondition != 1))
					{
						continue;
					}
				}
			}
			if (command.command.Contains("oem fb_mode_set") || command.command.Contains("oem fb_mode_clear") || base.SkipCommands.Exists((string n) => command.command.Contains(n)))
			{
				continue;
			}
			bool flag2 = false;
			if (base.IgnoreResultCommands.Exists((string n) => command.command.Contains(n)))
			{
				flag2 = true;
			}
			if (command.id == num)
			{
				flag = RunSubSteps();
			}
			if (!flag)
			{
				result = Result.FASTBOOT_FLASH_FAILED;
				break;
			}
			string text = EncapsulationFastbootCommand(command.command);
			empty = ProcessRunner.ProcessString(fullpath, text, command.timeout)?.ToLower();
			base.Log.AddLog("fastboot command : " + text + ", response: " + empty, upload: true);
			if (flag2)
			{
				if ((empty.Contains("fail") || empty.Contains("error")) && (command.command.ToLower().Contains("erase userdata") || command.command.ToLower().Contains("erase metadata")))
				{
					base.Recipe.IsEraseDataFailed = true;
				}
			}
			else if (empty.Contains("fail") || empty.Contains("error"))
			{
				failedResponse = "exec command failed: " + text + ", " + AnalysisFailedResponse(empty);
				if (empty.Contains("too many links"))
				{
					base.Recipe.UcDevice.MessageBox.Show(base.Name, "K1452", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
				}
				if (empty.Contains("failed to write") && !File.Exists(logPath))
				{
					ProcessRunner.ProcessString(fullpath, EncapsulationFastbootCommand("oem partition dump logfs"), 20000)?.ToLower();
					base.Log.AddLog("fastboot command: oem partition dump logfs, response: " + empty, upload: true);
				}
				result = ((!command.command.ToLower().Contains("erase userdata")) ? Result.FASTBOOT_FLASH_FAILED : Result.FASTBOOT_FLASH_ERASEDATE_FAILED);
				break;
			}
			if (totalDataSize > 0)
			{
				num2 += command.size;
				double progress = 100.0 * (double)num2 / (double)totalDataSize;
				ProgressUpdate(progress);
			}
		}
		closeResuingWndAction?.Invoke();
		closeResuingWndAction = null;
		return result;
	}

	private Result ParseCommands(string xml)
	{
		commands = new List<CommandLine>();
		totalDataSize = 0L;
		if (!File.Exists(xml))
		{
			failedResponse = $"fastboot xml file: {xml} not exists";
			base.Log.AddLog(failedResponse, upload: true);
			return Result.FASTBOOT_FLASH_FAILED;
		}
		string xml2 = File.ReadAllText(xml);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml2);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("/flashing/steps");
		string directoryName = Path.GetDirectoryName(xml);
		if (xmlNode != null)
		{
			XmlNodeList xmlNodeList = xmlNode.SelectNodes("step");
			if (xmlNodeList != null)
			{
				int num = 0;
				long num2 = 0L;
				foreach (XmlNode item in xmlNodeList)
				{
					num2 = 0L;
					XmlNode namedItem;
					string text = (((namedItem = item.Attributes.GetNamedItem("operation")) != null) ? namedItem.Value : string.Empty).Trim();
					string text2 = (((namedItem = item.Attributes.GetNamedItem("partition")) != null) ? namedItem.Value : string.Empty).Trim();
					string text3 = (((namedItem = item.Attributes.GetNamedItem("filename")) != null) ? namedItem.Value : string.Empty).Trim();
					string text4 = (((namedItem = item.Attributes.GetNamedItem("var")) != null) ? namedItem.Value : string.Empty).Trim();
					(((namedItem = item.Attributes.GetNamedItem("MD5")) != null) ? namedItem.Value : string.Empty).Trim();
					string text5;
					if (text3 != string.Empty)
					{
						text5 = Path.Combine(directoryName, text3);
						if (File.Exists(text5))
						{
							num2 = new FileInfo(text5).Length;
							totalDataSize += num2;
						}
					}
					else
					{
						text5 = string.Empty;
					}
					int timeout = ((!OperationToTimeout.ContainsKey(text)) ? 120000 : OperationToTimeout[text]);
					string text6 = ((!(text4 != string.Empty)) ? (string.IsNullOrEmpty(text5) ? (text + " " + text2) : (text + " " + text2 + " \"" + text5 + "\"")) : (text + " " + text4));
					text6 = text6.Trim();
					commands.Add(new CommandLine(++num, text6, timeout, num2));
				}
			}
		}
		return Result.PASSED;
	}
}
