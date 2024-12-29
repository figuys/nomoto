using System;

namespace lenovo.mbg.service.common.utilities;

[Serializable]
public class PluginInfo
{
	public string PluginID { get; set; }

	public string PluginName { get; set; }

	public string DisplayName { get; set; }

	public string Version { get; set; }

	public int VersionCode { get; set; }

	public string PluginDownloadUrl { get; set; }

	public string PluginIconPath { get; set; }

	public int Bits { get; set; }

	public int Order { get; set; }

	public string Description { get; set; }

	public bool haveNewVersion { get; set; }

	public bool Install { get; set; }

	public string PluginDir { get; set; }
}
