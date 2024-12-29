using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.socket;

[Serializable]
public class PropItem
{
	[JsonProperty("key")]
	public string Key { get; set; }

	[JsonProperty("value")]
	public string Value { get; set; }

	public override string ToString()
	{
		return "Key:" + Key + ",Value:" + Value;
	}
}
