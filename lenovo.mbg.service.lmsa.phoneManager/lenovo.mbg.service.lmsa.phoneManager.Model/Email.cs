using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class Email
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("email")]
	public string EmailAddr { get; set; }

	[JsonProperty("type")]
	public DetailType EmailType { get; set; }
}
