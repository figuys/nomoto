using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class ParamProperty
{
	[JsonProperty("label")]
	public string PropertyName { get; set; }

	[JsonProperty("property")]
	public string PropertyValue { get; set; }
}
