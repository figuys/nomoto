using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class ResourceModel
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("uri")]
	public string URI { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("unZip")]
	public bool UnZip { get; set; }

	[JsonProperty("md5")]
	public string MD5 { get; set; }

	[JsonProperty("clientBehavior")]
	public string ClientBehavior { get; set; }

	[JsonProperty("description")]
	public string Description { get; set; }

	public string UseExplan { get; set; }

	public int id { get; set; }

	public bool latest { get; set; }
}
