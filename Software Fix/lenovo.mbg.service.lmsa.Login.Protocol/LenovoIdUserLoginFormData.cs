using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class LenovoIdUserLoginFormData
{
	[JsonProperty("useName")]
	public string UserName { get; set; }

	[JsonProperty("wust")]
	public string WUST { get; set; }

	public string lenovoId { get; set; }
}
