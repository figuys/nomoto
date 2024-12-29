using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class UserPermissionResponseData
{
	[JsonProperty("parentid")]
	public string ParentId { get; set; }

	[JsonProperty("privName")]
	public string PrivName { get; set; }

	[JsonProperty("privId")]
	public string PrivId { get; set; }

	[JsonProperty("privType")]
	public string PrivType { get; set; }
}
