namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class RescueCollectionModel : RescueDeviceInfoModel
{
	public string startRescueTime { get; set; }

	public string rescueTime { get; set; }

	public int rescueResult { get; set; }

	public string clientUuid { get; set; }

	public bool automatched { get; set; }

	public string errorMsg { get; set; }

	public int romMatchType { get; set; }

	public string version { get; set; }

	public string description { get; set; }

	public string resultDescription { get; set; }

	public string failureCode { get; set; }

	public int? orderId { get; set; }

	public RescueCollectionModel()
	{
	}

	public RescueCollectionModel(RescueDeviceInfoModel deviceInfo)
	{
		if (deviceInfo != null)
		{
			base.brand = deviceInfo.brand;
			base.category = deviceInfo.category;
			base.sn = deviceInfo.sn;
			base.imei = deviceInfo.imei;
			base.memory = deviceInfo.memory;
			base.country = deviceInfo.country;
			base.simCount = deviceInfo.simCount;
			base.hwCode = deviceInfo.hwCode;
			base.modelName = deviceInfo.modelName;
			base.fingerPrint = deviceInfo.fingerPrint;
			base.roCarrier = deviceInfo.roCarrier;
			base.fsgVersion = deviceInfo.fsgVersion;
			base.blurVersion = deviceInfo.blurVersion;
			base.softwareVersion = deviceInfo.softwareVersion;
			base.rescueMark = deviceInfo.rescueMark;
			base.channelId = deviceInfo.channelId;
			base.cid = deviceInfo.cid;
		}
	}
}
