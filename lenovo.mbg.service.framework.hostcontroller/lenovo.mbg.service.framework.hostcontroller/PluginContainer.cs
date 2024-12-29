using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.framework.hostcontroller;

public class PluginContainer
{
	private AggregateCatalog mCatalog;

	private CompositionContainer mContainer;

	private ManualResetEvent resetEvent = new ManualResetEvent(initialState: false);

	private static PluginContainer _instance;

	[ImportMany(typeof(IPlugin), AllowRecomposition = true)]
	private IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }

	public static PluginContainer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new PluginContainer();
			}
			return _instance;
		}
	}

	private PluginContainer()
	{
		mCatalog = new AggregateCatalog();
		mContainer = new CompositionContainer(mCatalog);
	}

	public Task Init(List<string> plugindirs)
	{
		return Task.Run(delegate
		{
			plugindirs.ForEach(delegate(string n)
			{
				mCatalog.Catalogs.Add(new DirectoryCatalog(n));
			});
			mContainer.ComposeParts(this);
			resetEvent.Set();
		});
	}

	public void LoadPuginDir(List<string> plugindirs)
	{
		plugindirs.ForEach(delegate(string n)
		{
			mCatalog.Catalogs.Add(new DirectoryCatalog(n));
		});
		mContainer.ComposeParts(this);
		resetEvent.Set();
	}

	public Plugin Load(PluginInfo pluginInfo)
	{
		resetEvent.WaitOne();
		IPlugin plugin = GetPlugin(pluginInfo.PluginID);
		if (plugin != null)
		{
			return new Plugin(pluginInfo, plugin);
		}
		return null;
	}

	public IPlugin GetPlugin(string pluginId)
	{
		resetEvent.WaitOne();
		if (Plugins == null)
		{
			return null;
		}
		foreach (Lazy<IPlugin, IPluginMetadata> plugin in Plugins)
		{
			if (pluginId.Equals(plugin.Metadata.PluginId))
			{
				return plugin.Value;
			}
		}
		return null;
	}
}
