using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class ContactNote
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("note")]
	public string Note { get; set; }
}
