using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class DeviceModel
{
	public string id { get; set; }

	public bool IsUpload { get; set; }

	[JsonProperty("modelName")]
	public string ModelName { get; set; }

	[JsonProperty("imei")]
	public string IMEI { get; set; }

	[JsonProperty("imei2")]
	public string IMEI2 { get; set; }

	[JsonProperty("sn")]
	public string SN { get; set; }

	[JsonProperty("pn")]
	public string PN { get; set; }

	[JsonProperty("brand")]
	public string Brand { get; set; }

	[JsonProperty("category")]
	public string Category { get; set; }

	[JsonProperty("motoModelName")]
	public string MotoModelName { get; set; }
}