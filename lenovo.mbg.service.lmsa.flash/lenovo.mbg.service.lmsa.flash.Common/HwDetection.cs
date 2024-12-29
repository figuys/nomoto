using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.Common;

[Serializable]
public class HwDetection
{
	[JsonProperty("startTime")]
	public string StartTime { get; set; }

	[JsonProperty("finishTime")]
	public string FinishTime { get; set; }

	[JsonProperty("result")]
	public int Result { get; set; }

	[JsonProperty("model")]
	public string Model { get; set; }

	[JsonProperty("sn")]
	public string SN { get; set; }

	[JsonProperty("imei1")]
	public string Imei1 { get; set; }

	[JsonProperty("imei2")]
	public string Imei2 { get; set; }

	[JsonProperty("androidVersion")]
	public string AndroidVersion { get; set; }

	[JsonProperty("errorMessage")]
	public string ErrorMessage { get; set; }

	[JsonProperty("hwDetectionItems")]
	public List<HwDetectionItem> Items { get; set; }

	public string clientUuid { get; set; }
}
