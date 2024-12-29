namespace lenovo.mbg.service.lmsa.Business;

public class PluginCatalogInfo
{
	public string PluginID { get; set; }

	public string PluginName { get; set; }

	public string Version { get; set; }

	public string PluginDownloadUrl { get; set; }

	public string PluginDownloadName { get; set; }

	public long PluginDownloadSize { get; set; }

	public string PluginDownloadMD5 { get; set; }

	public string PluginAssemblyName { get; set; }

	public string PluginIconPath { get; set; }

	public string PluginUnIconPath { get; set; }

	public string Description { get; set; }

	public int Bits { get; set; }

	public bool Install { get; set; }

	public int VersionCode { get; set; }

	public bool ForceType { get; set; }
}
