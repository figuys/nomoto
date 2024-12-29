using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.support.Contract;

public class IBaseWarrantyServiceItemInfo
{
	[JsonProperty("channelId")]
	public string Channelid { get; set; }

	[JsonProperty("countryCode")]
	public string CountryCode { get; set; }

	[JsonProperty("countryName")]
	public string CountryDesc { get; set; }

	[JsonProperty("deliveryNumber")]
	public string DeliveryNbr { get; set; }

	[JsonProperty("geo")]
	public string Geo { get; set; }

	[JsonProperty("warrantyAttr")]
	public string WtyAttr { get; set; }

	[JsonProperty("sdf")]
	public string Sdf { get; set; }

	[JsonProperty("sdfDesc")]
	public string SdfDesc { get; set; }

	[JsonProperty("serviceDeliveryType")]
	public string ServiceDeliveryType { get; set; }

	[JsonProperty("shipDate")]
	public string ShipDate { get; set; }

	[JsonProperty("activationDate")]
	public string ActivationDate { get; set; }

	[JsonProperty("warrantyStartDate")]
	public string Warstart { get; set; }

	[JsonProperty("warrantyEndDate")]
	public string Wed { get; set; }

	[JsonProperty("warrantyCategory")]
	public string WarrantyCategory { get; set; }
}
