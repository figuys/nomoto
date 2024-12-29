using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phonemanager.apps.Model;

public class ResponseAppInfo
{
	[JsonProperty("systemapps")]
	public List<AppInfo> SystemAppInfos { get; set; }

	[JsonProperty("normalapps")]
	public List<AppInfo> NormalAppInfos { get; set; }
}
