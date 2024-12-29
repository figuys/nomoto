using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.tips;

public class SubSerierResponseModel
{
	[JsonProperty("content")]
	public string SubSerierName { get; set; }
}
