using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class LoggingInResponseData
{
	[JsonProperty("userId")]
	public string UserId { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("email")]
	public string EmailAddress { get; set; }

	[JsonProperty("country")]
	public string Country { get; set; }

	[JsonProperty("fullName")]
	public string FullName { get; set; }

	[JsonProperty("phone")]
	public string PhoneNumber { get; set; }

	[JsonProperty("isLenovoId")]
	public bool IsLenovoId { get; set; }

	[JsonProperty("notification")]
	public string RSANotification { get; set; }

	public Dictionary<string, object> dictionary { get; set; }

	[JsonProperty("b2bCountry")]
	public bool B2bCountry { get; set; }

	[JsonProperty("b2BUserInfo")]
	public B2BUserInfo B2bUsrInfo { get; set; }

	public int quitSurvey { get; set; }

	public int failureNum { get; set; }
}
