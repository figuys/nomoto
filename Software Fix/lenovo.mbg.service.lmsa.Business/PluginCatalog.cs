using System.Xml.Serialization;

namespace lenovo.mbg.service.lmsa.Business;

[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public class PluginCatalog
{
	private PluginCatalogPlugin[] pluginsField;

	[XmlArrayItem("Plugin", IsNullable = false)]
	public PluginCatalogPlugin[] Plugins
	{
		get
		{
			return pluginsField;
		}
		set
		{
			pluginsField = value;
		}
	}
}
