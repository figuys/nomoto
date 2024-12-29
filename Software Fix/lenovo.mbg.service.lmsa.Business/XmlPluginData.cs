using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;

namespace lenovo.mbg.service.lmsa.Business;

public class XmlPluginData : AbstractDataBase
{
	private string pluginXmlPath { get; set; }

	private string pluginsPath { get; set; }

	public XmlPluginData(string pluginXmlPath, string pluginsPath)
	{
		this.pluginXmlPath = pluginXmlPath;
		this.pluginsPath = pluginsPath;
	}

	public override List<PluginVersionModel> GetObject()
	{
		LogHelper.LogInstance.Info("lenovo.mbg.service.common.utilities.XmlPluginData: GetObject() Start");
		List<PluginVersionModel> list = new List<PluginVersionModel>();
		try
		{
			if (XmlSerializeHelper.DeserializeFromFile<PluginCatalog>(pluginXmlPath) is PluginCatalog { Plugins: not null, Plugins: var plugins })
			{
				foreach (PluginCatalogPlugin pluginCatalogPlugin in plugins)
				{
					list.Add(ConvertPluginCatalogPlugin2PluginVersionModel(pluginCatalogPlugin));
				}
				list = list.OrderBy((PluginVersionModel n) => n.Order).ToList();
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.common.utilities.XmlPluginData: GetObject() Exception", exception);
		}
		LogHelper.LogInstance.Info("lenovo.mbg.service.common.utilities.XmlPluginData: GetObject() End Success");
		return list;
	}

	public override void UpdateObject(List<PluginVersionModel> objs)
	{
		PluginCatalog pluginCatalog = new PluginCatalog();
		if (objs == null)
		{
			return;
		}
		List<PluginCatalogPlugin> list = new List<PluginCatalogPlugin>();
		foreach (PluginVersionModel obj in objs)
		{
			list.Add(ConvertPluginVersionModel2PluginCatalogPlugin(obj));
		}
		pluginCatalog.Plugins = list.ToArray();
		string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		string fileName = pluginXmlPath;
		try
		{
			XmlSerializeHelper.Serializer<PluginCatalog>(fileName, pluginCatalog);
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.common.utilities.XmlPluginData: UpdateObject() Exception", exception);
		}
	}

	private PluginVersionModel ConvertPluginCatalogPlugin2PluginVersionModel(PluginCatalogPlugin pluginCatalogPlugin)
	{
		return new PluginVersionModel
		{
			PluginID = pluginCatalogPlugin.PluginID,
			VersionName = pluginCatalogPlugin.PluginName,
			DisplayName = pluginCatalogPlugin.PluginNameKey,
			Version = pluginCatalogPlugin.Version,
			PluginAssemblyName = pluginCatalogPlugin.PluginAssemblyName,
			Bits = pluginCatalogPlugin.Bits,
			Description = pluginCatalogPlugin.Description,
			Install = pluginCatalogPlugin.Install,
			ForceType = pluginCatalogPlugin.ForceType,
			PluginIconPath = pluginCatalogPlugin.PluginIconPath,
			Order = pluginCatalogPlugin.Order,
			Valid = true
		};
	}

	private PluginCatalogPlugin ConvertPluginVersionModel2PluginCatalogPlugin(PluginVersionModel pluginVersionModel)
	{
		return new PluginCatalogPlugin
		{
			PluginID = pluginVersionModel.PluginID,
			PluginName = pluginVersionModel.VersionName,
			Version = pluginVersionModel.Version,
			PluginAssemblyName = pluginVersionModel.PluginAssemblyName,
			Bits = pluginVersionModel.Bits,
			Description = pluginVersionModel.Description,
			Install = pluginVersionModel.Install,
			ForceType = pluginVersionModel.ForceType,
			Order = pluginVersionModel.Order
		};
	}
}
