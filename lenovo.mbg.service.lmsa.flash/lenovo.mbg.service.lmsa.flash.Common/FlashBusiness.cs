using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.flash.ViewV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class FlashBusiness
{
	public const string SIMTAG_SINGLE = "Single";

	public const string SIMTAG_DUAL = "Dual";

	public const string Sku = "modelName";

	public const string BlurVersion = "blurVersion";

	public const string FingerPrint = "fingerPrint";

	public const string RoCarrier = "roCarrier";

	public const string FsgVersionQCOM = "fsgVersion.qcom";

	public const string FsgVersionMTK = "fsgVersion.mtk";

	public const string FsgVersionSAMSUNG = "fsgVersion.samsung";

	public const string SimCount = "simCount";

	public const string softwareVersion = "softwareVersion";

	public const string hwCode = "hwCode";

	public const string memory = "memory";

	public const string country = "country";

	public static Dictionary<ConnectType, Dictionary<string, string>> ConnectTypeToValue = new Dictionary<ConnectType, Dictionary<string, string>>
	{
		{
			ConnectType.Adb,
			AdbParamsToValue
		},
		{
			ConnectType.Fastboot,
			FastbootParamsToValue
		}
	};

	public static Dictionary<string, string> AdbParamsToValue = new Dictionary<string, string>
	{
		{ "modelName", "ro.boot.hardware.sku|ro.product.model" },
		{ "hwCode", "" },
		{ "softwareVersion", "ro.build.display.id" },
		{ "roCarrier", "ro.carrier" },
		{ "fsgVersion.qcom", "ril.baseband.config.version" },
		{ "fsgVersion.mtk", "ro.build.version.incremental" },
		{ "fsgVersion.samsung", "gsm.version.baseband" },
		{ "fingerPrint", "ro.build.fingerprint" },
		{ "blurVersion", "ro.build.version.full" },
		{ "simCount", "persist.radio.multisim.config" }
	};

	public static Dictionary<string, string> FastbootParamsToValue = new Dictionary<string, string>
	{
		{ "modelName", "sku" },
		{ "roCarrier", "ro.carrier" },
		{ "fsgVersion.qcom", "version-baseband" },
		{ "fsgVersion.mtk", "version-baseband" },
		{ "fsgVersion.samsung", "version-baseband" },
		{ "fingerPrint", "ro.build.fingerprint" },
		{ "softwareVersion", "softwareVersion" },
		{ "blurVersion", "ro.build.version.full" },
		{ "simCount", "oem hw dualsim" }
	};

	public static RescueDeviceInfoModel ConvertFastbootDeviceInfo(SortedList<string, string> infos)
	{
		RescueDeviceInfoModel rescueDeviceInfoModel = new RescueDeviceInfoModel();
		infos.TryGetValue("sku", out var value);
		rescueDeviceInfoModel.modelName = value;
		infos.TryGetValue("imei", out value);
		rescueDeviceInfoModel.imei = value;
		infos.TryGetValue("ro.build.fingerprint", out value);
		rescueDeviceInfoModel.fingerPrint = value;
		infos.TryGetValue("ro.carrier", out value);
		rescueDeviceInfoModel.roCarrier = value;
		infos.TryGetValue("ro.build.version.full", out value);
		rescueDeviceInfoModel.blurVersion = value;
		infos.TryGetValue("softwareVersion", out value);
		rescueDeviceInfoModel.softwareVersion = value;
		infos.TryGetValue("ram", out value);
		rescueDeviceInfoModel.memory = value;
		infos.TryGetValue("oem hw dualsim", out value);
		rescueDeviceInfoModel.simCount = value;
		infos.TryGetValue("serialno", out value);
		rescueDeviceInfoModel.sn = value;
		rescueDeviceInfoModel.brand = "Motorola";
		infos.TryGetValue("version-baseband", out value);
		rescueDeviceInfoModel.fsgVersion = value;
		infos.TryGetValue("fdr-allowed", out value);
		rescueDeviceInfoModel.fdr_allowed = value;
		if (!string.IsNullOrEmpty(value) && value.ToLower().Equals("no"))
		{
			rescueDeviceInfoModel.rescueMark = 1;
		}
		else
		{
			rescueDeviceInfoModel.rescueMark = 0;
		}
		infos.TryGetValue("rescuemark", out value);
		if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var result) && result > 0)
		{
			rescueDeviceInfoModel.rescueMark = result;
		}
		infos.TryGetValue("erase_personal_data", out value);
		rescueDeviceInfoModel.erase_personal_data = value;
		infos.TryGetValue("securestate", out value);
		rescueDeviceInfoModel.securestate = value;
		infos.TryGetValue("channelid", out value);
		rescueDeviceInfoModel.channelId = value;
		infos.TryGetValue("cid", out value);
		rescueDeviceInfoModel.cid = value;
		infos.TryGetValue("trackId", out value);
		rescueDeviceInfoModel.trackId = value;
		return rescueDeviceInfoModel;
	}

	public static void ConvertDeviceInfo(Dictionary<string, string> infos, RescueDeviceInfoModel deviceInfo)
	{
		foreach (KeyValuePair<string, string> info in infos)
		{
			if (info.Key == "blurVersion")
			{
				deviceInfo.blurVersion = info.Value;
			}
			else if (info.Key == "fingerPrint")
			{
				deviceInfo.fingerPrint = info.Value;
			}
			else if (info.Key == "roCarrier")
			{
				deviceInfo.roCarrier = info.Value;
			}
			else if (info.Key == "fsgVersion.qcom" || info.Key == "fsgVersion.mtk" || info.Key == "fsgVersion.samsung")
			{
				deviceInfo.fsgVersion = info.Value;
			}
			else if (info.Key == "simCount")
			{
				deviceInfo.simCount = info.Value;
			}
			else if (info.Key == "softwareVersion")
			{
				deviceInfo.softwareVersion = info.Value;
			}
			else if (info.Key == "hwCode")
			{
				deviceInfo.hwCode = info.Value;
			}
			else if (info.Key == "memory")
			{
				deviceInfo.memory = info.Value;
			}
			else if (info.Key == "country")
			{
				deviceInfo.country = info.Value;
			}
		}
	}

	public static T Copy<T>(T source, T target) where T : class
	{
		Type type = target.GetType();
		PropertyInfo[] properties = source.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			try
			{
				object value = propertyInfo.GetValue(source, null);
				object value2 = propertyInfo.GetValue(target, null);
				if (value != null && (value2 == null || string.IsNullOrEmpty(value2.ToString())) && (!propertyInfo.PropertyType.Equals(typeof(string)) || !string.IsNullOrEmpty(value.ToString())))
				{
					type.GetProperty(propertyInfo.Name).SetValue(target, value, null);
				}
			}
			catch (Exception)
			{
			}
		}
		return target;
	}

	public static Dictionary<string, string> GetParams(JArray aparams, DeviceEx device)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (device.ConnectType == ConnectType.Fastboot)
		{
			foreach (JToken aparam in aparams)
			{
				string text = aparam.ToString();
				string text2 = null;
				if (FastbootParamsToValue.Keys.Contains(text))
				{
					text2 = device.Property.GetPropertyValue(FastbootParamsToValue[text]);
					if (text == "fsgVersion.qcom" && !string.IsNullOrEmpty(text2))
					{
						if (text2.Contains("not found"))
						{
							text2 = null;
						}
						else
						{
							string[] array = Regex.Split(text2, "\\s");
							if (array.Length == 2)
							{
								text2 = array[1];
							}
						}
					}
					if (text == "blurVersion")
					{
						int.TryParse(device.Property.AndroidVersion, out var result);
						if (result >= 10)
						{
							continue;
						}
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = ((!(text == "simCount")) ? "-1" : "Lack");
				}
				dictionary.Add(text, text2?.Trim());
			}
		}
		else
		{
			string empty = string.Empty;
			foreach (JToken aparam2 in aparams)
			{
				string text3 = aparam2.ToString();
				if (text3 == "blurVersion")
				{
					int.TryParse(device.Property.AndroidVersion, out var result2);
					if (result2 >= 10)
					{
						continue;
					}
				}
				switch (text3)
				{
				case "hwCode":
					empty = device.Property.HWCode;
					break;
				case "modelName":
					empty = device.Property.ModelName;
					break;
				case "fsgVersion.qcom":
					empty = device.Property.GetPropertyValue(AdbParamsToValue[text3]);
					if (!string.IsNullOrWhiteSpace(empty))
					{
						break;
					}
					empty = device.Property.GetPropertyValue("gsm.version.baseband");
					if (!string.IsNullOrWhiteSpace(empty))
					{
						string[] array2 = Regex.Split(empty, "\\s");
						if (array2.Length == 2)
						{
							empty = array2[1];
						}
					}
					else
					{
						empty = device.Property.GetPropertyValue("vendor.ril.baseband.config.version");
					}
					break;
				case "simCount":
					empty = device.Property.GetPropertyValue(AdbParamsToValue[text3]);
					empty = ((!string.IsNullOrEmpty(empty) && !"SS".Equals(empty?.Trim(), StringComparison.CurrentCultureIgnoreCase) && !"ssss".Equals(empty?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ? "Dual" : "Single");
					break;
				default:
					empty = device.Property.GetPropertyValue(AdbParamsToValue[text3]);
					break;
				}
				if (string.IsNullOrEmpty(empty))
				{
					empty = "-1";
				}
				dictionary.Add(text3, empty?.Trim());
			}
		}
		return dictionary;
	}

	public static Dictionary<string, object> GetAutoMatchParams(DeviceEx device)
	{
		if (device != null)
		{
			string modelName = device.Property.ModelName;
			if (string.IsNullOrEmpty(modelName))
			{
				LogHelper.LogInstance.Info("device model name is Empty!");
				return null;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("modelName", modelName);
			LogHelper.LogInstance.Info("get match params by model name: " + modelName);
			JObject jObject = FlashContext.SingleInstance.service.RequestContent<JObject>(WebServicesContext.RESUCE_AUTOMATCH_GETPARAMS_MAPPING, dictionary);
			if (jObject == null)
			{
				LogHelper.LogInstance.Info("no match resource on the server!");
				return null;
			}
			if (jObject.Value<JArray>("params").Count > 0)
			{
				Dictionary<string, string> @params = GetParams(jObject.Value<JArray>("params"), device);
				@params.Add("category", device.Property.Category);
				LogHelper.LogInstance.Info("match rescue resource params: " + JsonHelper.SerializeObject2Json(@params));
				dictionary.Add("params", @params);
				dictionary.Add("imei", device.Property.IMEI1);
				dictionary.Add("imei2", device.Property.IMEI2);
				dictionary.Add("sn", device.Property.SN);
				string propertyValue = device.Property.GetPropertyValue("channelid");
				if (!string.IsNullOrEmpty(propertyValue))
				{
					dictionary.Add("channelId", propertyValue);
				}
				dictionary.Add("matchType", (device.ConnectType != ConnectType.Fastboot) ? 1 : 0);
				if (MainFrameV6.Instance.RomMatchId != null)
				{
					dictionary.Add("romMatchId", MainFrameV6.Instance.RomMatchId);
					MainFrameV6.Instance.RomMatchId = null;
				}
				return dictionary;
			}
		}
		return null;
	}
}
