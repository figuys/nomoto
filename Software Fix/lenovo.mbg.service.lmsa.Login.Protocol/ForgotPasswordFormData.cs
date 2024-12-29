using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class ForgotPasswordFormData
{
	[JsonProperty("name")]
	public string UserName { get; set; }

	[JsonProperty("email")]
	public string EmailAddress { get; set; }
}
