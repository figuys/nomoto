using System;
using System.Collections.ObjectModel;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.hostcontroller;

public class PluginController : IDisposable
{
	public ObservableCollection<Plugin> LoadedPlugins { get; private set; }

	public event EventHandler<PluginErrorEventArgs> OnPluginExecutingError;

	public PluginController()
	{
		LoadedPlugins = new ObservableCollection<Plugin>();
	}

	public void ClosePlugin(Plugin plugin)
	{
		if (plugin != null)
		{
			LoadedPlugins.Remove(plugin);
			DisposePlugin(plugin);
		}
	}

	public bool CanClose(Plugin plugin)
	{
		return plugin?.CanClose() ?? true;
	}

	public bool IsExecuteWork(Plugin plugin)
	{
		return plugin?.IsExecuteWork() ?? false;
	}

	public void Dispose()
	{
		foreach (Plugin loadedPlugin in LoadedPlugins)
		{
			DisposePlugin(loadedPlugin);
		}
	}

	public Plugin LoadPlugin(PluginInfo info)
	{
		Plugin plugin = null;
		try
		{
			plugin = PluginContainer.Instance.Load(info);
			if (plugin != null)
			{
				plugin.PluginError += OnPluginError;
				plugin.Init();
				plugin.CreateView();
				LoadedPlugins.Add(plugin);
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.hostcontroller.PluginController: LoadPlugin Exception.", exception);
			plugin = null;
		}
		return plugin;
	}

	private void OnPluginError(object sender, PluginErrorEventArgs args)
	{
	}

	private void PluginErrorHandler(PluginErrorEventArgs args)
	{
		LogHelper.LogInstance.Info("lenovo.mbg.service.framework.hostcontroller.PluginController: PluginErrorHandler Exception.");
		if (args != null && args.Plugin != null)
		{
			if (this.OnPluginExecutingError != null)
			{
				this.OnPluginExecutingError(this, args);
			}
			if (LoadedPlugins.Contains(args.Plugin))
			{
				LoadedPlugins.Remove(args.Plugin);
			}
			DisposePlugin(args.Plugin);
		}
	}

	private void DisposePlugin(Plugin plugin)
	{
		if (plugin == null)
		{
			return;
		}
		plugin.PluginError -= OnPluginError;
		try
		{
			plugin.Dispose();
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.hostcontroller.PluginController: DisposePlugin Exception.", exception);
		}
	}
}
