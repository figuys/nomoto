using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Contract;

[Serializable]
public class Plugin
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("pluginKey")]
	public string PluginKey { get; set; }

	[JsonProperty("remark")]
	public string Remark { get; set; }
}
