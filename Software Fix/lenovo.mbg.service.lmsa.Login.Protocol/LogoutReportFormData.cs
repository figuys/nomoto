using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class LogoutReportFormData
{
	[JsonProperty("userId")]
	public string UserId { get; set; }
}
