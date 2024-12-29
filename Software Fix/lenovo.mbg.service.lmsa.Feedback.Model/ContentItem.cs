using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Feedback.Model;

public class ContentItem
{
	[JsonProperty("sequence")]
	public int Sort { get; set; }

	[JsonProperty("value")]
	public string Content { get; set; }

	[JsonProperty("type")]
	public string DataType { get; set; }
}
