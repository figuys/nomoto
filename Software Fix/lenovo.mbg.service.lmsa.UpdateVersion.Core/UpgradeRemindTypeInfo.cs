using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Core;

public class UpgradeRemindTypeInfo
{
	[JsonProperty("currentVersion")]
	public string CurrentVersion { get; set; }

	[JsonProperty("newVersion")]
	public string NewVersion { get; set; }

	[JsonProperty("setDate")]
	public DateTime SetDate { get; set; }

	[JsonProperty("remindType")]
	public UpgradeRemindType RemindType { get; set; }
}
