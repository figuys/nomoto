using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class ParamPropertyWithValues
{
	[JsonProperty("paramProperty")]
	public ParamProperty ParamProperty { get; set; }

	[JsonProperty("paramValues")]
	public List<string> ParamValues { get; set; }
}
