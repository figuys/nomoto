using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

[Serializable]
public class SendSMSModel
{
	[JsonProperty("source")]
	public SMS Source { get; set; }

	[JsonProperty("target_address")]
	public List<string> TargetAddress { get; set; }
}
