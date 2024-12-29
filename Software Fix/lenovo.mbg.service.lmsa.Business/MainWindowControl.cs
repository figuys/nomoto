using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.hostcontroller;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;

namespace lenovo.mbg.service.lmsa.Business;

public class MainWindowControl : RaisePropertyBase
{
	public static PluginController m_pluginController;

	private Plugin m_selectedPlugin;

	private static volatile MainWindowControl _instance = null;

	private static readonly object lockHelper = new object();

	public static MainWindowControl Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (lockHelper)
				{
					if (_instance == null)
					{
						_instance = new MainWindowControl();
					}
				}
			}
			return _instance;
		}
	}

	public ObservableCollection<Plugin> LoadedPlugins => m_pluginController.LoadedPlugins;

	public Plugin SelectedPlugin
	{
		get
		{
			return m_selectedPlugin;
		}
		set
		{
			m_selectedPlugin = value;
			RaisePropertyChanged("SelectedPlugin");
			OnSelected(string.Empty);
		}
	}

	public bool CanExit
	{
		get
		{
			if (LoadedPlugins != null && LoadedPlugins.Count > 0)
			{
				foreach (Plugin loadedPlugin in LoadedPlugins)
				{
					if (loadedPlugin.IsExecuteWork())
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	private List<PluginInfo> m_AvailablePlugins { get; set; }

	public event EventHandler<PluginErrorEventArgs> OnPluginError;

	private MainWindowControl()
	{
		m_pluginController = new PluginController();
		m_pluginController.OnPluginExecutingError += FirePluginExecutingError;
	}

	public List<PluginInfo> LoadPlguinInfo()
	{
		List<PluginInfo> list = new List<PluginInfo>();
		object @object = ApplcationClass.PluginDataBase.GetObject();
		if (@object != null)
		{
			List<PluginVersionModel> list2 = (List<PluginVersionModel>)@object;
			foreach (PluginVersionModel item in list2)
			{
				list.Add(new PluginInfo
				{
					PluginID = item.PluginID,
					PluginName = item.VersionName,
					DisplayName = item.DisplayName,
					Version = item.Version,
					PluginDownloadUrl = item.downloadUrl,
					PluginDir = Path.Combine(ApplcationClass.PluginsPath, item.PluginID),
					PluginIconPath = item.PluginIconPath,
					Bits = item.Bits,
					Description = item.Description,
					Install = item.Install,
					haveNewVersion = item.haveNewVersion,
					Order = item.Order
				});
			}
			m_AvailablePlugins = list;
		}
		return list;
	}

	public bool CanClose(Plugin plugin)
	{
		bool result = true;
		try
		{
			result = m_pluginController.CanClose(plugin);
		}
		catch (Exception)
		{
		}
		return result;
	}

	public bool IsExecuteWork(Plugin plugin)
	{
		bool result = false;
		try
		{
			result = m_pluginController.IsExecuteWork(plugin);
		}
		catch (Exception)
		{
		}
		return result;
	}

	public bool IsExecuteWork()
	{
		return m_pluginController.LoadedPlugins.Any((Plugin n) => n.IsExecuteWork());
	}

	public void OnSelected(string val)
	{
		SelectedPlugin?.OnSelected(val);
	}

	public void ClosePlugin(Plugin plugin)
	{
		m_pluginController.ClosePlugin(plugin);
	}

	public void LoadPluginCallback(Action<Plugin> selectedPlugin, PluginInfo info)
	{
		Plugin plugin = null;
		if (LoadedPlugins != null && LoadedPlugins.Count > 0)
		{
			foreach (Plugin loadedPlugin in LoadedPlugins)
			{
				if (loadedPlugin.PluginInfo.PluginID.Equals(info.PluginID))
				{
					plugin = loadedPlugin;
					break;
				}
			}
		}
		if (plugin == null)
		{
			plugin = m_pluginController.LoadPlugin(info);
		}
		SelectedPlugin = plugin;
		selectedPlugin(plugin);
	}

	public Plugin GetPluginbyPluginInfo(PluginInfo info)
	{
		if (LoadedPlugins != null && LoadedPlugins.Count > 0)
		{
			return LoadedPlugins.Where((Plugin p) => p.PluginInfo.PluginID.Equals(info.PluginID)).First();
		}
		return null;
	}

	private void FirePluginExecutingError(object sender, PluginErrorEventArgs e)
	{
		if (this.OnPluginError == null)
		{
			return;
		}
		Delegate[] invocationList = this.OnPluginError.GetInvocationList();
		foreach (Delegate @delegate in invocationList)
		{
			try
			{
				((EventHandler<PluginErrorEventArgs>)@delegate)(sender, e);
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.Business.MainWindowControl.FirePluginExecutingError: Plugin Execute Exception", exception);
			}
		}
	}

	public void Dispose()
	{
		try
		{
			if (m_pluginController != null)
			{
				m_pluginController.OnPluginExecutingError -= FirePluginExecutingError;
				m_pluginController.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}
}
