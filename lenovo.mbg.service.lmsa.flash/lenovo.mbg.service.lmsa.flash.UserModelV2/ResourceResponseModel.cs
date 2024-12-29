using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class ResourceResponseModel
{
	[JsonProperty("category")]
	public string Category { get; set; }

	[JsonProperty("brand")]
	public string Brand { get; set; }

	[JsonProperty("hwcode")]
	public string HWCode { get; set; }

	[JsonProperty("modelName")]
	public string ModelName { get; set; }

	[JsonProperty("realModelName")]
	public string RealModelName { get; set; }

	[JsonProperty("saleModel")]
	public string SalesModel { get; set; }

	[JsonProperty("flashFlow")]
	public string RecipeResource { get; set; }

	[JsonProperty("paramProperty")]
	public ParamProperty ParamProperty { get; set; }

	[JsonProperty("paramValues")]
	public List<string> ParamValues { get; set; }

	[JsonProperty("iconResource")]
	public ResourceModel IconResource { get; set; }

	[JsonProperty("toolResource")]
	public ResourceModel ToolResource { get; set; }

	[JsonProperty("romResource")]
	public ResourceModel RomResources { get; set; }

	[JsonProperty("countryCodeResource")]
	public ResourceModel CountryCode { get; set; }

	[JsonProperty("backUpPopup")]
	public bool IsShowBackupTip { get; set; }

	[JsonProperty("platform")]
	public string Platform { get; set; }

	public string fingerprint { get; set; }

	public string marketName { get; set; }

	public bool latest { get; set; }

	public string latestDesc { get; set; }

	public string comments { get; set; }

	public string romMatchId { get; set; }

	public bool fastboot { get; set; }

	[JsonProperty("paramProperties")]
	public List<ParamPropertyWithValues> ParamProperties { get; set; }
}
