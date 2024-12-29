using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Contract;

[Serializable]
public class ResourceResponseModel
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("needExternalLink")]
	public bool NeedExternalLink { get; set; }

	[JsonProperty("plugin")]
	public Plugin Plugin { get; set; }

	[JsonProperty("externalUrl")]
	public string ExternalUrl { get; set; }

	[JsonProperty("iconResource")]
	public ResourceModel IconResource { get; set; }

	[JsonProperty("sort")]
	public int Sort { get; set; }

	[JsonProperty("image")]
	public ResourceModel Image { get; set; }

	[JsonProperty("icon")]
	public ResourceModel Icon { get; set; }
}
