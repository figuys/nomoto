using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.ModelV6;

public class MotoCareInfo
{
	public string country { get; set; }

	public string url { get; set; }

	public string thumbnailImg { get; set; }

	public string picture { get; set; }

	public int type { get; set; }

	public int id { get; set; }

	public string sn { get; set; }

	[JsonIgnore]
	public string imei { get; set; }

	[JsonIgnore]
	public string WarrantyStartDate { get; set; }

	[JsonIgnore]
	public string WarrantyEndDate { get; set; }

	[JsonIgnore]
	public bool InWarranty { get; set; }
}
