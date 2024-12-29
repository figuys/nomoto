using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

[Serializable]
public class AndroidDeviceProperty : IAndroidDevice, ILoadDeviceData
{
	private PropInfo _PropInfo;

	protected List<string> countryElements = new List<string> { "ro.lenovo.easyimage.code", "persist.sys.withsim.country", "gsm.operator.iso-country" };

	private string _imei1 = string.Empty;

	private string _imei2 = string.Empty;

	private string _pn = string.Empty;

	private string _sn = string.Empty;

	private string _internalStoragePath = string.Empty;

	private string _externalStoragePath = string.Empty;

	private string _totalExternalStorageWithUnit = string.Empty;

	private string _usedExternalStorageWithUnit = string.Empty;

	private string _totalInternalStorageWithUnit = string.Empty;

	private string _usedInternalStorageWithUnit = string.Empty;

	private string _freeExternalStorageWithUnit = string.Empty;

	private string _freeInternalStorageWithUnit = string.Empty;

	private long _totalExternalStorage;

	private long _usedExternalStorage;

	private long _totalInternalStorage;

	private long _usedInternalStorage;

	private long _freeExternalStorage;

	private long _freeInternalStorage;

	private string _processor = string.Empty;

	private string _uptime = string.Empty;

	public static readonly string[] SN_PROP_FIELDS = new string[7] { "sys.customsn.showcode", "ro.lenovosn2", "persist.radio.factory_phone_sn", "gsm.lenovosn2", "persist.sys.snvalue", "ro.serialno", "ro.odm.lenovo.sn" };

	public static readonly string[] SN_PROP_INVALID_VALUES = new string[1] { "UNKNOWN" };

	protected static Dictionary<string, List<string>> propsMapping = new Dictionary<string, List<string>>
	{
		{
			"imei1",
			new List<string> { "device.imei1", "gsm.imei1" }
		},
		{
			"imei2",
			new List<string> { "device.imei2", "gsm.imei2" }
		},
		{
			"sn",
			new List<string> { "sys.customsn.showcode", "ro.lenovosn2", "persist.radio.factory_phone_sn", "gsm.lenovosn2", "persist.sys.snvalue", "ro.serialno", "ro.odm.lenovo.sn" }
		},
		{
			"pn",
			new List<string> { "gsm.serial", "gsm.sn1", "ro.psnno", "sys.pn", "ro.pcbasn", "persist.sys.cit.sn", "gsm.sn", "persist.sys.pnvalue", "ro.odm.lenovo.psn", "sys.pcba.serialno" }
		}
	};

	public string AndroidVersion => _PropInfo.GetProp("ro.build.version.release");

	public string Brand => _PropInfo.GetProp("ro.product.brand");

	public string Carrier => _PropInfo.GetProp("ro.carrier");

	public string CountryCode
	{
		get
		{
			string text = null;
			foreach (string countryElement in countryElements)
			{
				text = _PropInfo.GetProp(countryElement);
				if (text != null)
				{
					break;
				}
			}
			return text;
		}
	}

	public string FingerPrint => _PropInfo.GetProp("ro.build.fingerprint");

	public string HWCode => GetHwCode();

	public string IMEI1
	{
		get
		{
			string text = DeviceReadConfig.Instance[ModelNameEx]?.imei;
			List<string> list = propsMapping["imei1"];
			if (!string.IsNullOrEmpty(text))
			{
				list = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Union(list).ToList();
			}
			string text2 = null;
			foreach (string item in list)
			{
				text2 = _PropInfo.GetProp(item);
				if (!string.IsNullOrEmpty(text2))
				{
					break;
				}
			}
			return text2;
		}
		private set
		{
			_imei1 = value;
		}
	}

	public string IMEI2
	{
		get
		{
			string text = DeviceReadConfig.Instance[ModelNameEx]?.imei2;
			List<string> list = propsMapping["imei2"];
			if (!string.IsNullOrEmpty(text))
			{
				list = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Union(list).ToList();
			}
			string text2 = null;
			foreach (string item in list)
			{
				text2 = _PropInfo.GetProp(item);
				if (!string.IsNullOrEmpty(text2))
				{
					break;
				}
			}
			return text2;
		}
		private set
		{
			_imei2 = value;
		}
	}

	public string ModelName
	{
		get
		{
			string result = _PropInfo.GetProp("ro.product.model");
			if (!string.IsNullOrEmpty(Brand) && Regex.IsMatch(Brand, "motorola", RegexOptions.IgnoreCase))
			{
				string prop = _PropInfo.GetProp("ro.boot.hardware.sku");
				if (!string.IsNullOrEmpty(prop))
				{
					result = prop;
				}
			}
			return result;
		}
	}

	public string ModelName2 => _PropInfo.GetProp("ro.product.model");

	protected string ModelNameEx
	{
		get
		{
			string text = _PropInfo.GetProp("ro.product.model");
			if (!string.IsNullOrEmpty(Brand) && Regex.IsMatch(Brand, "motorola", RegexOptions.IgnoreCase))
			{
				string prop = _PropInfo.GetProp("ro.boot.hardware.sku");
				if (!string.IsNullOrEmpty(prop))
				{
					text = prop;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "UnKnown";
		}
	}

	public string OtaModel => _PropInfo.GetProp("ro.product.ota.model");

	public string ModelId => string.Empty;

	[JsonIgnore]
	public Dictionary<string, string> Others
	{
		get
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			if (_PropInfo.Props != null)
			{
				_PropInfo.Props.ForEach(delegate(PropItem n)
				{
					dic.Add(n.Key, n.Value);
				});
			}
			return dic;
		}
	}

	public string PN
	{
		get
		{
			if (string.IsNullOrEmpty(_pn))
			{
				_pn = GetPN();
			}
			return _pn;
		}
		private set
		{
			_pn = value;
		}
	}

	public string RomVersion => _PropInfo.GetProp("ro.build.version.incremental");

	public string CustomerVersion => _PropInfo.GetProp("ro.build.customer-version");

	public string RoHardWare => _PropInfo.GetProp("ro.hardware");

	public string SN
	{
		get
		{
			if (string.IsNullOrEmpty(_sn))
			{
				_sn = GetSN();
			}
			return _sn;
		}
		private set
		{
			_sn = value;
		}
	}

	public double BatteryQuantityPercentage
	{
		get
		{
			double result = 0.0;
			double.TryParse(_PropInfo.GetProp("battery.quantity"), out result);
			return result;
		}
	}

	public string InternalStoragePath
	{
		get
		{
			if (!string.IsNullOrEmpty(_internalStoragePath))
			{
				return _internalStoragePath;
			}
			_internalStoragePath = _PropInfo.GetProp("Internal.Storage.Path");
			return _internalStoragePath;
		}
	}

	public string ExternalStoragePath
	{
		get
		{
			if (!string.IsNullOrEmpty(_externalStoragePath))
			{
				return _externalStoragePath;
			}
			_externalStoragePath = _PropInfo.GetProp("External.Storage.Path");
			return _externalStoragePath;
		}
	}

	public string TotalExternalStorageWithUnit
	{
		get
		{
			if (string.IsNullOrEmpty(_totalExternalStorageWithUnit))
			{
				string prop = _PropInfo.GetProp("External.Storage.TotalWithUnit");
				_totalExternalStorageWithUnit = prop;
			}
			return _totalExternalStorageWithUnit;
		}
	}

	public string UsedExternalStorageWithUnit
	{
		get
		{
			if (string.IsNullOrEmpty(_usedExternalStorageWithUnit))
			{
				string prop = _PropInfo.GetProp("External.Storage.UsedWithUnit");
				_usedExternalStorageWithUnit = prop;
			}
			return _usedExternalStorageWithUnit;
		}
	}

	public string TotalInternalStorageWithUnit => GlobalFun.ConvertLong2String(TotalInternalStorage, "F0");

	public string UsedInternalStorageWithUnit => GlobalFun.ConvertLong2String(UsedInternalStorage, "F0");

	public string FreeExternalStorageWithUnit
	{
		get
		{
			if (string.IsNullOrEmpty(_freeExternalStorageWithUnit))
			{
				string prop = _PropInfo.GetProp("External.Storage.FreeWithUnit");
				_freeExternalStorageWithUnit = prop;
			}
			return _freeExternalStorageWithUnit;
		}
	}

	public string FreeInternalStorageWithUnit
	{
		get
		{
			long num = TotalInternalStorage - UsedInternalStorage;
			if (num < 0)
			{
				num = 0L;
			}
			return GlobalFun.ConvertLong2String(num, "F0");
		}
	}

	public long TotalExternalStorage
	{
		get
		{
			if (_totalExternalStorage == 0L)
			{
				string prop = _PropInfo.GetProp("External.Storage.Total");
				if (!string.IsNullOrEmpty(prop))
				{
					long result = 0L;
					long.TryParse(prop, out result);
					return _totalExternalStorage = result;
				}
			}
			return _totalExternalStorage;
		}
	}

	public long UsedExternalStorage
	{
		get
		{
			if (_usedExternalStorage == 0L)
			{
				string prop = _PropInfo.GetProp("External.Storage.Used");
				if (!string.IsNullOrEmpty(prop))
				{
					long result = 0L;
					long.TryParse(prop, out result);
					return _usedExternalStorage = result;
				}
			}
			return _usedExternalStorage;
		}
	}

	public long TotalInternalStorage
	{
		get
		{
			if (_totalInternalStorage == 0L)
			{
				string prop = _PropInfo.GetProp("Internal.Storage.Total");
				if (!string.IsNullOrEmpty(prop))
				{
					long result = 0L;
					long.TryParse(prop, out result);
					return _totalInternalStorage = result;
				}
			}
			return _totalInternalStorage;
		}
	}

	public long UsedInternalStorage
	{
		get
		{
			if (_usedInternalStorage == 0L)
			{
				string prop = _PropInfo.GetProp("Internal.Storage.Used");
				if (!string.IsNullOrEmpty(prop))
				{
					long result = 0L;
					long.TryParse(prop, out result);
					return _usedInternalStorage = result;
				}
			}
			return _usedInternalStorage;
		}
	}

	public long FreeExternalStorage
	{
		get
		{
			if (_freeExternalStorage == 0L)
			{
				string prop = _PropInfo.GetProp("External.Storage.Free");
				if (!string.IsNullOrEmpty(prop))
				{
					long result = 0L;
					long.TryParse(prop, out result);
					return _freeExternalStorage = result;
				}
			}
			return _freeExternalStorage;
		}
	}

	public long FreeInternalStorage
	{
		get
		{
			if (_freeInternalStorage == 0L)
			{
				string prop = _PropInfo.GetProp("Internal.Storage.Free");
				if (!string.IsNullOrEmpty(prop))
				{
					long result = 0L;
					long.TryParse(prop, out result);
					return _freeInternalStorage = result;
				}
			}
			return _freeInternalStorage;
		}
	}

	public string Operator => _PropInfo.GetProp("phone.type");

	public string FsgVersion
	{
		get
		{
			string prop = _PropInfo.GetProp("ril.baseband.config.version");
			if (string.IsNullOrEmpty(prop))
			{
				prop = _PropInfo.GetProp("gsm.version.baseband");
			}
			if (string.IsNullOrEmpty(prop))
			{
				prop = _PropInfo.GetProp("vendor.ril.baseband.config.version");
			}
			return prop;
		}
	}

	public string Processor
	{
		get
		{
			if (string.IsNullOrEmpty(_processor))
			{
				_processor = _PropInfo.GetProp("processor");
			}
			return _processor;
		}
	}

	public string Uptime
	{
		get
		{
			if (string.IsNullOrEmpty(_uptime))
			{
				_uptime = _PropInfo.GetProp("upTime");
			}
			return _uptime;
		}
	}

	public int ApiLevel => _PropInfo.GetIntProp("ro.build.version.sdk");

	public string Category
	{
		get
		{
			string modelName = ModelName;
			if ("Lenovo PB2-690Y".Equals(modelName, StringComparison.OrdinalIgnoreCase) || "Lenovo PB2-690M".Equals(modelName, StringComparison.OrdinalIgnoreCase))
			{
				return "tablet";
			}
			List<string> source = new List<string> { "phone", "tablet" };
			string _category = _PropInfo.GetProp("ro.lenovo.device");
			if (string.IsNullOrEmpty(_category) || !source.Contains(_category, StringComparer.CurrentCultureIgnoreCase))
			{
				_category = _PropInfo.GetProp("ro.build.characteristics");
			}
			if (string.IsNullOrEmpty(_category) || !source.Contains(_category, StringComparer.CurrentCultureIgnoreCase))
			{
				_category = _PropInfo.GetProp("ro.odm.lenovo.device");
			}
			return source.FirstOrDefault((string n) => n.Equals(_category, StringComparison.CurrentCultureIgnoreCase));
		}
	}

	public int SimCount
	{
		get
		{
			string prop = _PropInfo.GetProp("persist.radio.multisim.config");
			if (string.IsNullOrEmpty(prop) || "SS".Equals(prop?.Trim(), StringComparison.CurrentCultureIgnoreCase) || "ssss".Equals(prop?.Trim(), StringComparison.CurrentCultureIgnoreCase))
			{
				return 1;
			}
			return 2;
		}
	}

	public AndroidDeviceProperty()
	{
		_PropInfo = new PropInfo();
	}

	public AndroidDeviceProperty(PropInfo propInfo)
	{
		_PropInfo = propInfo;
	}

	public void Load()
	{
	}

	public void Load(PropInfo prop)
	{
		if (prop != null)
		{
			_PropInfo.AddOrUpdateProp(prop.Props);
		}
	}

	public void AddOrUpdate(PropItem prop)
	{
		_PropInfo.AddOrUpdateProp(prop);
	}

	private string GetPN()
	{
		string text = DeviceReadConfig.Instance[ModelNameEx]?.pn;
		List<string> list = propsMapping["pn"];
		if (!string.IsNullOrEmpty(text))
		{
			list = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Union(list).ToList();
		}
		foreach (string item in list)
		{
			string text2 = _PropInfo.GetProp(item);
			if (string.IsNullOrEmpty(text2))
			{
				continue;
			}
			if (text2.Contains(" "))
			{
				string[] array = text2.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0)
				{
					text2 = array[0];
				}
			}
			if (text2.Length == 18 || text2.Length == 23 || text2.Length == 25)
			{
				return text2.Trim();
			}
		}
		return string.Empty;
	}

	private string GetSN()
	{
		string text = DeviceReadConfig.Instance[ModelNameEx]?.sn;
		List<string> list = propsMapping["sn"];
		if (!string.IsNullOrEmpty(text))
		{
			list = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Union(list).ToList();
		}
		string empty = string.Empty;
		if (string.IsNullOrEmpty(empty))
		{
			foreach (string item in list)
			{
				empty = _PropInfo.GetProp(item);
				if (!string.IsNullOrEmpty(empty) && !SN_PROP_INVALID_VALUES.Contains(empty.ToUpper()))
				{
					return empty.Trim();
				}
			}
		}
		return string.Empty;
	}

	private string GetHwCode()
	{
		if (string.IsNullOrEmpty(PN))
		{
			return string.Empty;
		}
		string empty = string.Empty;
		return (PN.Length switch
		{
			18 => PN.Substring(3, 2), 
			23 => PN.Substring(14, 2), 
			25 => (ModelName == null || !ModelName.Replace(" ", "").Equals("LenovoA2020a40", StringComparison.CurrentCultureIgnoreCase)) ? PN.Substring(23, 2) : PN.Substring(14, 2), 
			_ => string.Empty, 
		}).Trim();
	}

	public string GetPropertyValue(string name)
	{
		return _PropInfo.GetProp(name);
	}
}
