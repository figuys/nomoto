using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class Address
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("address")]
	public string AddressStr { get; set; }

	[JsonProperty("type")]
	public DetailType AddressType { get; set; }
}
