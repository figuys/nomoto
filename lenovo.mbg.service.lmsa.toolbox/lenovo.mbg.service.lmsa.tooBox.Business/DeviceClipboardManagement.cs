using System.Collections.Generic;
using System.Diagnostics;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.Business;

namespace lenovo.mbg.service.lmsa.tooBox.Business;

public class DeviceClipboardManagement : IDeviceClipboard
{
	public string GetClipboardInfo()
	{
		string result = string.Empty;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		BusinessData businessData = new BusinessData(BusinessType.CLIP_BOARD_EXPORT, tcpAndroidDevice);
		if (tcpAndroidDevice == null)
		{
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.CLIP_BOARD_EXPORT, businessData.Update(stopwatch.ElapsedMilliseconds, BusinessStatus.FALIED, null));
			return result;
		}
		if (tcpAndroidDevice.Property.ApiLevel >= 29 && tcpAndroidDevice.ConnectType != ConnectType.Wifi && !tcpAndroidDevice.DeviceOperator.Command("shell dumpsys \"window | grep mFocusedApp\"", -1, tcpAndroidDevice.Identifer).Contains("com.lmsa.lmsaappclient"))
		{
			tcpAndroidDevice.FocuseApp();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return result;
		}
		if (messageReaderAndWriter == null)
		{
			return result;
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		bool flag = false;
		if (messageReaderAndWriter.SendAndReceiveSync("getDeviceClipboardInfo", "getDeviceClipboardInfoResponse", new List<string>(), sequence, out receiveData) && receiveData != null && receiveData != null && receiveData[1].Contains("success"))
		{
			flag = true;
			result = GlobalFun.DecodeBase64(receiveData[2]);
		}
		stopwatch.Stop();
		HostProxy.BehaviorService.Collect(BusinessType.CLIP_BOARD_EXPORT, businessData.Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
		return result;
	}

	public bool ImportClipboardInfo(string ClipBoardContent)
	{
		bool result = false;
		List<PropItem> receiveData = null;
		if (!string.IsNullOrEmpty(ClipBoardContent))
		{
			List<string> parameter = new List<string> { "text/plain", ClipBoardContent };
			if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
			{
				return false;
			}
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			long sequence = HostProxy.Sequence.New();
			if (messageReaderAndWriter.SendAndReceiveSync("importClipboardInfo", "importClipboardInfoResponse", parameter, sequence, out receiveData) && receiveData != null)
			{
				result = receiveData.Exists((PropItem m) => "success".Equals(m.Value));
			}
		}
		return result;
	}
}
