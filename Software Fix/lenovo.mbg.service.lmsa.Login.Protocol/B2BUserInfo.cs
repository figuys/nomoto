using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class B2BUserInfo
{
	[JsonProperty("vipName")]
	public string B2bMode { get; set; }

	[JsonProperty("multiDevice")]
	public bool IsMultiDev { get; set; }

	[JsonProperty("display")]
	public bool B2bButtonDisplay { get; set; }
}
