using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class StepTipsResponseModel
{
	[JsonProperty("key")]
	public string TipsKey { get; set; }

	[JsonProperty("value")]
	public string TipsContent { get; set; }

	[JsonProperty("description")]
	public string TipsTitle { get; set; }
}
