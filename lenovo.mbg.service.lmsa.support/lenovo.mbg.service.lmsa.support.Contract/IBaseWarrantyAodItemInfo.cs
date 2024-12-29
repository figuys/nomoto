using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyAodItemInfo
{
	[JsonProperty("aodType")]
	public string AodType { get; set; }

	[JsonProperty("aodDescription")]
	public string AodDescription { get; set; }
}
