namespace lenovo.mbg.service.lmsa.UpdateVersion.Model;

public class PluginVersionModel : VersionModel
{
	public string PluginID { get; set; }

	public string PluginAssemblyName { get; set; }

	public string Description { get; set; }

	public int Bits { get; set; }

	public bool Install { get; set; }

	public int Order { get; set; }

	public bool Valid { get; set; }

	public string PluginIconPath { get; set; }
}
