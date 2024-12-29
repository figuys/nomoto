using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class RescueDeviceInfoModel
{
	public string brand { get; set; }

	public string category { get; set; }

	public string sn { get; set; }

	public string imei { get; set; }

	public string memory { get; set; }

	public string country { get; set; }

	public string simCount { get; set; }

	public string hwCode { get; set; }

	[JsonIgnore]
	public string marketName { get; set; }

	public string modelName { get; set; }

	public string fingerPrint { get; set; }

	public string roCarrier { get; set; }

	public string fsgVersion { get; set; }

	public string blurVersion { get; set; }

	public string softwareVersion { get; set; }

	public int rescueMark { get; set; }

	[JsonIgnore]
	public string securestate { get; set; }

	public string cid { get; set; }

	public string channelId { get; set; }

	public string romMatchId { get; set; }

	public string trackId { get; set; }

	public string fdr_allowed { get; set; }

	public string erase_personal_data { get; set; }

	[JsonIgnore]
	public string saleModel { get; set; }
}
