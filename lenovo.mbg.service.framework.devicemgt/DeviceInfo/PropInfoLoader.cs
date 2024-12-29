using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class PropInfoLoader
{
	private PropInfo _PropInfo;

	public PropInfoLoader()
	{
		_PropInfo = new PropInfo();
	}

	public PropInfo LoadAll(TcpAndroidDevice device)
	{
		using (MessageReaderAndWriter msgRWHander = device.MessageManager.CreateMessageReaderAndWriter())
		{
			LoadProp(msgRWHander);
			LoadIMEI(msgRWHander);
			LoadSN(msgRWHander);
			LoadBattery(msgRWHander);
			LoadInternalStorageInfo(msgRWHander);
			LoadExternalStroageInfo(msgRWHander);
			LoadProcessor(msgRWHander);
			LoadUpTime(msgRWHander);
			if (device.ConnectType == ConnectType.Adb)
			{
				LoadPropByAdb(device);
			}
		}
		return _PropInfo;
	}

	private void LoadPropByAdb(TcpAndroidDevice device)
	{
		string text = AdbOperator.Instance.Command("shell getprop", -1, device.Identifer);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		MatchCollection matchCollection = Regex.Matches(text, "\\[(?<key>.*)\\]:\\s+?\\[(?<value>.*)\\]", RegexOptions.Multiline);
		if (matchCollection == null || matchCollection.Count <= 0)
		{
			return;
		}
		List<PropItem> list = new List<PropItem>();
		foreach (Match item in matchCollection)
		{
			list.Add(new PropItem
			{
				Key = item.Groups["key"].Value.Trim(),
				Value = item.Groups["value"].Value?.Trim()
			});
		}
		_PropInfo.AddOrUpdateProp(list);
	}

	private void LoadProp(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		List<string> first = new List<string>
		{
			"ro.build.version.release", "ro.product.brand", "ro.carrier", "ro.lenovo.easyimage.code", "persist.sys.withsim.country", "ro.build.fingerprint", "gsm.imei1", "gsm.imei2", "ro.odm.lenovo.psn", "ro.odm.lenovo.sn",
			"ro.product.model", "ro.product.ota.model", "ro.build.version.incremental", "ro.build.customer-version", "ro.build.characteristics", "ro.lenovo.device", "ro.hardware", "phone.type", "gsm.serial", "gsm.sn1",
			"ro.psnno", "sys.pn", "ro.pcbasn", "persist.sys.cit.sn", "gsm.sn", "persist.sys.pnvalue", "ro.build.version.sdk", "ro.build.display.id", "ro.boot.hardware.sku", "ril.baseband.config.version",
			"gsm.version.baseband", "vendor.ril.baseband.config.version", "ro.build.version.full", "persist.radio.multisim.config", "sys.customsn.showcode", "ro.lenovosn2", "persist.radio.factory_phone_sn", "gsm.lenovosn2", "persist.sys.snvalue", "ro.serialno",
			"sys.pcba.serialno"
		};
		List<string> allProps = DeviceReadConfig.Instance.GetAllProps();
		List<string> parameter = first.Union(allProps).ToList();
		if (msgRWHander.SendAndReceiveSync("getProp", "getPropResponse", parameter, Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData);
		}
	}

	private void LoadIMEI(MessageReaderAndWriter msgRWHander)
	{
		string text = string.Empty;
		string empty = string.Empty;
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getIMEI1", "getIMEI1Response", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData, new Dictionary<string, string> { { "imei1", "device.imei1" } });
			text = _PropInfo.GetProp("device.imei1");
			if (!string.IsNullOrEmpty(text) && "NULL".Equals(text.ToUpper()))
			{
				text = string.Empty;
				_PropInfo.Reset("device.imei1", text);
			}
		}
		List<PropItem> receiveData2 = null;
		if (msgRWHander.SendAndReceiveSync("getIMEI2", "getIMEI2Response", new List<string>(), Sequence.SingleInstance.New(), out receiveData2))
		{
			_PropInfo.AddOrUpdateProp(receiveData2, new Dictionary<string, string> { { "imei2", "device.imei2" } });
			empty = _PropInfo.GetProp("device.imei2");
			if (!string.IsNullOrEmpty(empty) && "NULL".Equals(empty.ToUpper()))
			{
				empty = string.Empty;
				_PropInfo.Reset("device.imei2", empty);
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(empty) && text.Equals(empty))
			{
				_PropInfo.Reset("device.imei2", string.Empty);
			}
		}
	}

	private void LoadBattery(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getBattery", "getBatteryResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData, new Dictionary<string, string> { { "battery", "battery.quantity" } });
		}
	}

	private void LoadInternalStorageInfo(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getInternalStorageInfo", "getInternalStorageInfoResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData, new Dictionary<string, string>
			{
				{ "path", "Internal.Storage.Path" },
				{ "total", "Internal.Storage.Total" },
				{ "used", "Internal.Storage.Used" },
				{ "free", "Internal.Storage.Free" },
				{ "totalWithUnit", "Internal.Storage.TotalWithUnit" },
				{ "usedWithUnit", "Internal.Storage.UsedWithUnit" },
				{ "freeWithUnit", "Internal.Storage.FreeWithUnit" }
			});
		}
	}

	private void LoadExternalStroageInfo(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getExternalStorageInfo", "getExternalStorageInfoResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData, new Dictionary<string, string>
			{
				{ "path", "External.Storage.Path" },
				{ "total", "External.Storage.Total" },
				{ "used", "External.Storage.Used" },
				{ "free", "External.Storage.Free" },
				{ "totalWithUnit", "External.Storage.TotalWithUnit" },
				{ "usedWithUnit", "External.Storage.UsedWithUnit" },
				{ "freeWithUnit", "External.Storage.FreeWithUnit" }
			});
		}
	}

	private void LoadProcessor(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getProcessor", "getProcessorResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData);
		}
	}

	private void LoadUpTime(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getUpTime", "getUpTimeResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData);
		}
	}

	private void LoadSN(MessageReaderAndWriter msgRWHander)
	{
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getSN", "getSNResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData))
		{
			_PropInfo.AddOrUpdateProp(receiveData);
		}
	}
}
