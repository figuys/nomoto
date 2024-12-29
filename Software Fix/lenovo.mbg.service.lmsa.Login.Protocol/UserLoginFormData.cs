using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class UserLoginFormData
{
	public static string TAG => "Storage_UserLoginFormData";

	[JsonProperty("userSource")]
	public string UserSource { get; set; }

	[JsonProperty("userData")]
	public string UserData { get; set; }
}
