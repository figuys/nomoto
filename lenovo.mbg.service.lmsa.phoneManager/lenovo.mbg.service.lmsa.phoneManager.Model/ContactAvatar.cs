using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class ContactAvatar
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("thumbnailFIlePath")]
	public string ThumbnailFIlePath { get; set; }
}
