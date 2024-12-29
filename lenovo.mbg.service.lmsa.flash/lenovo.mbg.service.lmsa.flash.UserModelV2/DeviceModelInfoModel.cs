using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class DeviceModelInfoModel
{
	[JsonProperty("category")]
	public string Category { get; set; }

	[JsonProperty("marketName")]
	public string MarketName { get; set; }

	[JsonProperty("modelName")]
	public string ModelName { get; set; }

	[JsonProperty("brand")]
	public string Brand { get; set; }

	[JsonProperty("platform")]
	public string PlatForm { get; set; }

	[JsonProperty("readSupport")]
	public bool ReadSupport { get; set; }

	[JsonProperty("readFlow")]
	public string Recipe { get; set; }
}
