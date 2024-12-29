using System;
using System.ComponentModel.Composition;

namespace lenovo.mbg.service.framework.services;

[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginExportAttribute : ExportAttribute
{
	public string PluginId { get; private set; }

	public PluginExportAttribute(Type contractType, string pluginId)
		: base(contractType)
	{
		PluginId = pluginId;
	}
}
