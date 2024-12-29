using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.Common;

[Serializable]
public class HwDetectionItem
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("result")]
	public int Result { get; set; }

	[JsonProperty("errorMessage")]
	public string ErrorMessage { get; set; }
}
