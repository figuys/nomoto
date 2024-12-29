using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class PicServerAlbumInfo
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("name")]
	public string AlbumName { get; set; }

	[JsonProperty("picsCount")]
	public int FileCount { get; set; }

	[JsonProperty("path")]
	public string Path { get; set; }
}
