using System;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class AutoMatchResource
{
	public DeviceEx device { get; set; }

	public RescueDeviceInfoModel deviceInfo { get; set; }

	public ResourceResponseModel resource { get; set; }

	public MatchInfo matchInfo { get; set; }

	public string Id { get; set; }

	public AutoMatchResource(DeviceEx device, RescueDeviceInfoModel deviceInfo, ResourceResponseModel resource, MatchInfo matchInfo)
	{
		this.device = device;
		this.deviceInfo = deviceInfo;
		this.resource = resource;
		this.matchInfo = matchInfo;
		if (device != null)
		{
			Id = device.Identifer;
			return;
		}
		if (!string.IsNullOrEmpty(deviceInfo.imei))
		{
			Id = deviceInfo.imei;
			return;
		}
		if (!string.IsNullOrEmpty(deviceInfo.sn))
		{
			Id = deviceInfo.sn;
			return;
		}
		string text = DateTime.Now.ToString("MMddHHmmss") + "#";
		if (!string.IsNullOrEmpty(deviceInfo.marketName))
		{
			text = text + deviceInfo.marketName + "#";
		}
		if (!string.IsNullOrEmpty(deviceInfo.modelName))
		{
			text = text + deviceInfo.modelName + "#";
		}
		if (!string.IsNullOrEmpty(deviceInfo.hwCode))
		{
			text = text + deviceInfo.hwCode + "#";
		}
		if (!string.IsNullOrEmpty(deviceInfo.country))
		{
			text = text + deviceInfo.country + "#";
		}
		if (!string.IsNullOrEmpty(deviceInfo.simCount))
		{
			text = text + deviceInfo.simCount + "#";
		}
		if (!string.IsNullOrEmpty(deviceInfo.memory))
		{
			text = text + deviceInfo.memory + "#";
		}
		Id = text.TrimEnd('#');
	}
}
