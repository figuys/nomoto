using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class LmsaUserLoginFormData
{
	[JsonProperty("password")]
	public string Password { get; set; }

	[JsonProperty("email")]
	public string EmailAddress { get; set; }
}
