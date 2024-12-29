using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class Phone
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("number")]
	public string PhoneNumber { get; set; }

	[JsonProperty("type")]
	public DetailType PhoneType { get; set; }
}
