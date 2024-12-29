using System.Xml.Serialization;

namespace lenovo.mbg.service.lmsa.Business;

[XmlType(AnonymousType = true)]
public class PluginCatalogPlugin
{
	private string pluginIDField;

	private string pluginNameField;

	private string versionField;

	private string pluginDownloadUrlField;

	private string pluginDownloadNameField;

	private string pluginDownloadSizeField;

	private string pluginDownloadMD5Field;

	private string pluginAssemblyNameField;

	private string pluginIconPathField;

	private string pluginNameKeyField;

	private string descriptionField;

	private bool installField;

	private bool forceTypeField;

	private int bitsField;

	private int orderField;

	[XmlAttribute]
	public string PluginID
	{
		get
		{
			return pluginIDField;
		}
		set
		{
			pluginIDField = value;
		}
	}

	[XmlAttribute]
	public string PluginNameKey
	{
		get
		{
			return pluginNameKeyField;
		}
		set
		{
			pluginNameKeyField = value;
		}
	}

	[XmlAttribute]
	public string PluginName
	{
		get
		{
			return pluginNameField;
		}
		set
		{
			pluginNameField = value;
		}
	}

	[XmlAttribute]
	public string Version
	{
		get
		{
			return versionField;
		}
		set
		{
			versionField = value;
		}
	}

	[XmlAttribute]
	public string PluginDownloadUrl
	{
		get
		{
			return pluginDownloadUrlField;
		}
		set
		{
			pluginDownloadUrlField = value;
		}
	}

	[XmlAttribute]
	public string PluginDownloadName
	{
		get
		{
			return pluginDownloadNameField;
		}
		set
		{
			pluginDownloadNameField = value;
		}
	}

	[XmlAttribute]
	public string PluginDownloadSize
	{
		get
		{
			return pluginDownloadSizeField;
		}
		set
		{
			pluginDownloadSizeField = value;
		}
	}

	[XmlIgnore]
	public long DownloadSize
	{
		get
		{
			long result = 0L;
			long.TryParse(pluginDownloadSizeField, out result);
			return result;
		}
		set
		{
			pluginDownloadSizeField = value.ToString();
		}
	}

	[XmlAttribute]
	public string PluginDownloadMD5
	{
		get
		{
			return pluginDownloadMD5Field;
		}
		set
		{
			pluginDownloadMD5Field = value;
		}
	}

	[XmlAttribute]
	public string PluginAssemblyName
	{
		get
		{
			return pluginAssemblyNameField;
		}
		set
		{
			pluginAssemblyNameField = value;
		}
	}

	[XmlAttribute]
	public string PluginIconPath
	{
		get
		{
			return pluginIconPathField;
		}
		set
		{
			pluginIconPathField = value;
		}
	}

	[XmlAttribute]
	public string Description
	{
		get
		{
			return descriptionField;
		}
		set
		{
			descriptionField = value;
		}
	}

	[XmlAttribute]
	public bool Install
	{
		get
		{
			return installField;
		}
		set
		{
			installField = value;
		}
	}

	[XmlAttribute]
	public bool ForceType
	{
		get
		{
			return forceTypeField;
		}
		set
		{
			forceTypeField = value;
		}
	}

	[XmlAttribute]
	public int Bits
	{
		get
		{
			return bitsField;
		}
		set
		{
			bitsField = value;
		}
	}

	[XmlAttribute]
	public int Order
	{
		get
		{
			return orderField;
		}
		set
		{
			orderField = value;
		}
	}
}
