using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class UserRegisterFormData
{
	[JsonProperty("name")]
	public string UserName { get; set; }

	[JsonProperty("fullName")]
	public string FullName { get; set; }

	[JsonProperty("password")]
	public string Password { get; set; }

	[JsonProperty("email")]
	public string EmailAddress { get; set; }

	[JsonProperty("phone")]
	public string PhoneNumber { get; set; }

	[JsonProperty("country")]
	public string Country { get; set; }

	[JsonProperty("city")]
	public string City { get; set; }

	[JsonProperty("address")]
	public string HouseAddress { get; set; }

	[JsonProperty("company")]
	public string Company { get; set; }
}
