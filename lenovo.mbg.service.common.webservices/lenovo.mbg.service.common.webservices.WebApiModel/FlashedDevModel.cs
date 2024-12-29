using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

public class FlashedDevModel
{
	[JsonProperty("imei")]
	public string Imei { get; set; }

	[JsonProperty("modelName")]
	public string ModelName { get; set; }

	[JsonProperty("category")]
	public string Category { get; set; }

	[JsonProperty("createDate")]
	public long? createDate { get; set; }

	public DateTime FlashDate => new DateTime(1970, 1, 1).AddMilliseconds(createDate.Value);
}
