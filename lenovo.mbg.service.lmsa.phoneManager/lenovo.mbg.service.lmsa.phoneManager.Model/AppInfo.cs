using System.Windows.Media;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class AppInfo
{
	private ImageSource Icon { get; set; }

	[JsonProperty("packageName")]
	public string PackageName { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("size")]
	public string Size { get; set; }

	[JsonProperty("dataUse")]
	public string DataUse { get; set; }

	[JsonProperty("currentVersion")]
	public string Version { get; set; }

	[JsonProperty("isSystem")]
	public bool IsSystem { get; set; }

	public string icon { get; set; }
}
