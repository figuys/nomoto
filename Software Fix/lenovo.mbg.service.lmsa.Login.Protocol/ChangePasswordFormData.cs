using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class ChangePasswordFormData
{
	[JsonProperty("password")]
	public string OldPassword { get; set; }

	[JsonProperty("newPwd")]
	public string NewPassword { get; set; }

	[JsonProperty("userId")]
	public string UserId { get; set; }
}
