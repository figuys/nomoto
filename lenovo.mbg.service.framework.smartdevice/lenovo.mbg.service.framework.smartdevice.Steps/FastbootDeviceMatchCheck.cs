using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootDeviceMatchCheck : BaseStep
{
	private readonly List<string> elements = new List<string>
	{
		"sku", "serialno", "imei", "ro.carrier", "version-baseband", "ro.build.fingerprint", "ro.build.version.full", "ro.build.version.qcom", "oem hw dualsim", "softwareVersion",
		"ram", "emmc", "androidVer", "fdr-allowed", "securestate", "cid"
	};

	private DeviceEx device;

	private IAndroidDevice properties;

	public override void Run()
	{
		if (base.TimeoutMilliseconds <= 0)
		{
			base.TimeoutMilliseconds = 20000;
		}
		if (Retry <= 0)
		{
			Retry = 1;
		}
		do
		{
			properties = FindDevice();
			if (properties != null)
			{
				break;
			}
			int retry = Retry - 1;
			Retry = retry;
		}
		while (Retry >= 0);
		if (properties == null)
		{
			base.Recipe.UcDevice.MessageBox.Show("K0071", (device == null) ? "K0935" : "K1478", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
			base.Log.AddResult(this, Result.QUIT, (device == null) ? "Not find fastboot device!" : "Read device info failed");
			return;
		}
		bool? flag = null;
		if (base.Info.Args.EXE != null && base.Info.Args.Command != null)
		{
			string text = base.Info.Args.EXE;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = $"\"{(object)base.Cache[key]}\"";
			}
			string text2 = base.Info.Args.Command;
			if (base.Info.Args.Format != null)
			{
				List<object> list = new List<object>();
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
			int timeout = base.Info.Args.Timeout ?? ((object)6000);
			flag = BatFileCheckDowngrade(text + " " + text2, timeout);
		}
		else
		{
			flag = NewMethodCheckDowngrade();
		}
		string text4 = null;
		string text5 = null;
		string text6 = null;
		string text7 = null;
		string text8 = null;
		string text9 = null;
		string text10 = null;
		if (properties != null)
		{
			foreach (string element in elements)
			{
				string text11 = properties.GetPropertyValue(element);
				if (element == "ram" && !string.IsNullOrEmpty(text11))
				{
					text11 = Regex.Split(text11, "\\s")[0];
				}
				base.Log.AddInfo(element, text11);
			}
			text4 = properties.ModelName;
			text5 = properties.IMEI1;
			text6 = properties.GetPropertyValue("softwareVersion");
			text7 = base.Log.Info["fdr-allowed"];
			text8 = base.Log.Info["securestate"];
			text10 = base.Log.Info["cid"];
			text9 = properties.GetPropertyValue("channelid");
			base.Log.AddLog("modelname: " + text4 + ", imei: " + text5 + ", softwareVersion: " + text6 + ", securestate: " + text8 + ",fdrallowed: " + text7 + ",channelid: " + text9 + " ", upload: true);
		}
		bool flag2 = ErrorRulesCheck(properties);
		string value = base.Info.Args.ErrorRulePromptText;
		string value2 = base.Info.Args.OnlyCheckModelName ?? "False";
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "onlyCheckModelName", value2 },
			{
				"tractId",
				base.Recipe.Device.Identifer
			},
			{ "modelname", text4 },
			{ "imei", text5 },
			{ "softwareVersion", text6 },
			{ "fdrallowed", text7 },
			{ "securestate", text8 },
			{
				"versioncheck",
				$"{flag}"
			},
			{
				"channelid",
				text9 ?? ""
			},
			{
				"cid",
				text10 ?? ""
			},
			{
				"errorRules",
				flag2.ToString()
			},
			{ "errorRuleMessage", value }
		};
		Task<object> task = base.Log.NotifyAsync(RecipeMessageType.MODELNAME, data);
		task.Wait();
		int num = (int)task.Result;
		Result result = Result.QUIT;
		string response = null;
		switch (num)
		{
		case 0:
		{
			bool num2 = base.Info.Args.ExitWithFailed ?? ((object)false);
			base.RunResult = "MODELNAME_CHECK_FAILED";
			response = "modelname check failed";
			result = ((!num2) ? Result.MODELNAME_CHECK_FAILED_QUIT : Result.FAILED);
			FreeDevice();
			base.Log.AddLog("==========modelname unmatch, reset recipe device: null ==========");
			if (base.Info.Args.PromptText != null)
			{
				base.Recipe.UcDevice.MessageBox.Show("K0711", string.Format(HostProxy.LanguageService.Translate(base.Info.Args.PromptText.ToString()), text4)).Wait();
			}
			break;
		}
		case 1:
			response = "device downgrade";
			result = Result.FASTBOOT_DEGRADE_QUIT;
			break;
		case 2:
			response = "fdr-allowed: no";
			break;
		case 3:
			response = "securestate: flashing_locked";
			break;
		case 5:
			response = "Cid:" + text10 + ", device is in states been modified somehow";
			result = Result.FASTBOOT_CID_CHECKE_QUIT;
			break;
		case 6:
			response = "data match error rule";
			result = Result.FASTBOOT_ERROR_RULES_QUIT;
			break;
		default:
			result = Result.PASSED;
			break;
		}
		base.Log.AddResult(this, result, response);
	}

	private IAndroidDevice FindDevice()
	{
		DateTime now = DateTime.Now;
		do
		{
			device = GetDevice(ConnectType.Fastboot, (object d) => (d as DeviceEx).SoftStatus == DeviceSoftStateEx.Online);
			if (device != null)
			{
				return device.Property;
			}
			Thread.Sleep(500);
		}
		while (DateTime.Now.Subtract(now).TotalMilliseconds < (double)base.TimeoutMilliseconds);
		return null;
	}

	private void FreeDevice()
	{
		if (base.Recipe.UcDevice.ManualDevice)
		{
			base.Recipe.UcDevice.Device.WorkType = DeviceWorkType.None;
			base.Recipe.UcDevice.Device = null;
		}
	}

	private int? GetVersionFormFile(string fileName)
	{
		string[] array = File.ReadAllLines(fileName);
		foreach (string text in array)
		{
			if (!text.Contains("HAB_SECURITY_VERSION"))
			{
				continue;
			}
			string[] array2 = text.Trim().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			try
			{
				if (array2.Length >= 2)
				{
					return Convert.ToInt32(array2[1]);
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}
		return null;
	}

	private (bool, int?) GetDevVersion(string cmdResp)
	{
		if (string.IsNullOrEmpty(cmdResp))
		{
			return (false, null);
		}
		bool item = false;
		int? item2 = null;
		string[] array = cmdResp.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		string empty = string.Empty;
		string[] array2 = array;
		foreach (string text in array2)
		{
			empty = text.Trim(' ', '.').ToLower();
			if (empty.Contains("_da"))
			{
				item = true;
			}
			if (!empty.Contains("vbmeta") && !text.Contains("RIL #0"))
			{
				continue;
			}
			string[] array3 = empty.Split(new string[1] { " = " }, StringSplitOptions.RemoveEmptyEntries);
			if (array3.Length == 2)
			{
				try
				{
					item2 = ((!array3[1].StartsWith("0x")) ? new int?(Convert.ToInt32(array3[1])) : new int?(Convert.ToInt32(array3[1], 16)));
				}
				catch (Exception)
				{
				}
			}
		}
		return (item, item2);
	}

	private bool? NewMethodCheckDowngrade()
	{
		string text = base.Resources.Get("Rom");
		if (string.IsNullOrEmpty(text))
		{
			base.Log.AddLog("Rom Directory is null");
			return null;
		}
		if (!Directory.Exists(text))
		{
			base.Log.AddLog("Rom Directory: " + text + ", not exists");
			return null;
		}
		string text2 = Path.Combine(text, "signing-info.txt");
		if (File.Exists(text2))
		{
			int? versionFormFile = GetVersionFormFile(text2);
			base.Log.AddLog("signing-info.txt rom version is [" + ((!versionFormFile.HasValue) ? "null" : versionFormFile.ToString()) + "]!", upload: true);
			if (versionFormFile.HasValue)
			{
				string text3 = EncapsulationFastbootCommand("oem read_sv");
				text3 = "fastboot " + text3;
				string text4 = ProcessRunner.CmdExcuteWithExit(text3, null, 6000);
				base.Log.AddLog("check downgrade command: " + text3 + ", response: " + text4);
				if (string.IsNullOrEmpty(text4))
				{
					base.Log.AddLog("shell cmd [fastboot oem read_sv] have no response!", upload: true);
					return null;
				}
				(bool, int?) devVersion = GetDevVersion(text4);
				base.Log.AddLog($"Device is MTK {devVersion.Item1}, device version is {devVersion.Item2}!", upload: true);
				if (devVersion.Item2.HasValue)
				{
					return versionFormFile < devVersion.Item2;
				}
			}
		}
		return null;
	}

	private bool? BatFileCheckDowngrade(string commandLine, int timeout)
	{
		if (!File.Exists(Path.Combine(base.Resources.Get("Rom"), "sign_info.txt")))
		{
			return null;
		}
		string text = ProcessRunner.CmdExcuteWithExit(commandLine, base.Resources.Get("Rom"), timeout);
		if (!string.IsNullOrEmpty(text))
		{
			int? num = null;
			int? num2 = null;
			string[] array = text.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			_ = string.Empty;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2.Contains("device rollback id sum:"))
				{
					string[] array3 = text2.Trim().Split(':');
					if (array3.Length == 2)
					{
						int result = 0;
						if (int.TryParse(array3[1], out result))
						{
							num2 = result;
						}
					}
				}
				if (!text2.Contains("signinfo id sum:"))
				{
					continue;
				}
				string[] array4 = text2.Trim().Split(':');
				if (array4.Length == 2)
				{
					int result2 = 0;
					if (int.TryParse(array4[1], out result2))
					{
						num = result2;
					}
				}
			}
			if (!num2.HasValue || !num.HasValue || num == 0)
			{
				return null;
			}
			if (base.Info.Args.SVValueMax != null)
			{
				int num3 = base.Info.Args.SVValueMax;
				if (num2 > num3)
				{
					base.Log.AddLog($"Bat file read device version is {num2}, larger than SVValueMax:[{num3}]! signing-info.txt rom version is {num}.", upload: true);
					return null;
				}
			}
			base.Log.AddLog($"Bat file read device version is {num2}, signing-info.txt rom version is {num}!", upload: true);
			return num < num2;
		}
		base.Log.AddLog("Bat file excute timeout!", upload: true);
		return null;
	}

	private bool ErrorRulesCheck(IAndroidDevice properties)
	{
		if (base.Info.Args.ErrorRules is JArray { HasValues: not false } jArray)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (JToken item3 in jArray)
			{
				List<string> list = new List<string>();
				string key = item3.Value<string>("Property");
				JToken jToken = item3.Value<JToken>("Values");
				if (jToken is JArray jArray2)
				{
					list = jArray2.Values<string>().ToList();
				}
				else
				{
					string item = jToken.Value<string>();
					list.Add(item);
				}
				if (list != null && list.Count > 0)
				{
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, list);
					}
					else
					{
						dictionary[key].AddRange(list);
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (base.Info.Args.ErrorExtendRules is JArray { HasValues: not false } jArray3)
			{
				bool flag = true;
				foreach (string item2 in jArray3.Values<string>().ToList())
				{
					KeyValuePair<string, List<string>> keyValuePair = dictionary.FirstOrDefault((KeyValuePair<string, List<string>> m) => m.Key.Equals(item2));
					if (keyValuePair.Value != null)
					{
						string propertyValue = properties.GetPropertyValue(item2);
						flag = checkFun(keyValuePair.Value, propertyValue);
						if (!flag)
						{
							stringBuilder.AppendLine(propertyValue + " unmatch error rule: " + string.Join(",", keyValuePair.Value));
							break;
						}
						stringBuilder.AppendLine(propertyValue + " match error rule: " + string.Join(",", keyValuePair.Value));
					}
				}
				base.Log.AddLog(stringBuilder.ToString(), upload: true);
				return flag;
			}
			bool flag2 = true;
			foreach (KeyValuePair<string, List<string>> item4 in dictionary)
			{
				string propertyValue2 = properties.GetPropertyValue(item4.Key);
				flag2 = checkFun(item4.Value, propertyValue2);
				if (flag2)
				{
					stringBuilder.AppendLine(propertyValue2 + " match error rule: " + string.Join(",", item4.Value));
					break;
				}
				stringBuilder.AppendLine(propertyValue2 + " unmatch error rule: " + string.Join(",", item4.Value));
			}
			base.Log.AddLog(stringBuilder.ToString(), upload: true);
			return flag2;
		}
		return false;
		static bool checkFun(List<string> ruleValueList, string data)
		{
			return ruleValueList.Exists(delegate(string r)
			{
				if (string.IsNullOrEmpty(r))
				{
					return string.IsNullOrEmpty(data);
				}
				return !string.IsNullOrEmpty(data) && Regex.IsMatch(data, r, RegexOptions.IgnoreCase);
			});
		}
	}
}
