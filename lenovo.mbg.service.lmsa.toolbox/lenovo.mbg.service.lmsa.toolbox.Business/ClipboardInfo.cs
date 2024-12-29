using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.toolbox.Business;

internal class ClipboardInfo
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("content")]
	public string Content { get; set; }

	[JsonProperty("size")]
	public string Size { get; set; }
}
