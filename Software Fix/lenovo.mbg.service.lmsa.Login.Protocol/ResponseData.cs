using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class ResponseData<T>
{
	[JsonProperty("code")]
	public string Code { get; set; }

	[JsonProperty("desc")]
	public string Description { get; set; }

	[JsonProperty("content")]
	public T Data { get; set; }
}
