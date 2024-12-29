using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyUpmaItemInfo
{
	[JsonProperty("mStartDate")]
	public string StartDate { get; set; }

	[JsonProperty("mEndDate")]
	public string EndDate { get; set; }

	[JsonProperty("mSDF")]
	public string Sdf { get; set; }

	[JsonProperty("mSDFDesc")]
	public string SdfDesc { get; set; }

	[JsonProperty("mSDFType")]
	public string SdfType { get; set; }

	[JsonProperty("remainWtyCount")]
	public string RemainWtyCount { get; set; }
}
