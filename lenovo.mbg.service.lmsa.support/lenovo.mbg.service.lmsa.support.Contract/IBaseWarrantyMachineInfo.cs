using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyMachineInfo
{
	[JsonProperty("buildDate")]
	public string BuildDate { get; set; }

	[JsonProperty("modelNumber")]
	public string Model { get; set; }

	[JsonProperty("productId")]
	public string Product { get; set; }

	[JsonProperty("productName")]
	public string ProductName { get; set; }

	[JsonProperty("serialNumber")]
	public string Serial { get; set; }

	[JsonProperty("status")]
	public string Status { get; set; }

	[JsonProperty("machineType")]
	public string Type { get; set; }

	[JsonProperty("imei")]
	public string IMEI { get; set; }

	[JsonProperty("popDate")]
	public string POPDate { get; set; }

	[JsonProperty("swapImei")]
	public string SWAPIMEI { get; set; }
}
