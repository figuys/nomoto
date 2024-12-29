using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.tips;

public class SubSerierRequestModel
{
	[JsonProperty("modelName")]
	public string ModelName { get; set; }

	[JsonProperty("brandType")]
	public string Brand { get; set; }
}
