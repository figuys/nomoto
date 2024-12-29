using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

[Serializable]
public class ToolVersionModel
{
	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("clientVersion")]
	public string VersionNumber { get; set; }

	[JsonProperty("filePath")]
	public string FilePath { get; set; }

	[JsonProperty("fileSize")]
	public long FileSize { get; set; }

	[JsonProperty("md5")]
	public string MD5 { get; set; }

	[JsonProperty("forceUpdate")]
	public bool IsForce { get; set; }

	[JsonProperty("releaseDate")]
	public string ReleaseDate { get; set; }

	[JsonProperty("releaseNotes")]
	public string ReleaseNotes { get; set; }
}
