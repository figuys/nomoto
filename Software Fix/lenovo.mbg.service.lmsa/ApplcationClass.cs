using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using GoogleAnalytics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.hostcontroller;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;
using lenovo.mbg.service.lmsa.UserControls.VersionUpdateControls.PlugInsUpdate;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa;

public class ApplcationClass
{
	private static volatile ApplcationClass _instance = null;

	private static readonly object lockHelper = new object();

	private static XmlPluginData m_xmlPluginData = null;

	private static readonly string s_PluginFlashScreenName = "lmsa-plugin-Flash";

	private static readonly string s_PluginDeviceScreenName = "lmsa-plugin-Device-main";

	private static readonly string s_PluginSupportScreenName = "lmsa-plugin-Support-main";

	private static readonly string s_PluginForumScreenName = "lmsa-plugin-Forum";

	private static readonly string s_PluginMessengerScreenName = "lmsa-plugin-Messenger";

	private static readonly string s_PluginTipsScreenName = "lmsa-plugin-Tips";

	private static readonly string s_PluginToolboxScreenName = "lmsa-plugin-Toolbox-main";

	private static readonly string s_PluginDefaultScreenName = "lmsa-host";

	public static bool ForceUpdate = false;

	public static bool isAddPlugIning = false;

	private static bool frameHasNewVersion = false;

	private static Plugin lastSelectedPlugin = null;

	private static UpdateTipWindow updateTip = null;

	private static PlugInPanelModel plugInPanelModelCurrentSelected = null;

	private static PlugInPanelModel plugInPanelModelCurrentUpdated = null;

	private static bool isUpdatingPlug;

	public static MainWindow ApplcationStartWindow { get; set; }

	public static PlugInPanel PlugInPanel { get; set; }

	public static ContentPresenter ContentPresenterPlugin { get; set; }

	public static PopupEx NonTopmostPopup { get; set; }

	public static IPlugin CurrentPlugin { get; private set; }

	public static bool manualTrigger { get; set; }

	public static List<PluginModel> AvailablePlugins { get; set; }

	public static DownloadControlViewModel DownloadViewModel { get; set; }

	public static string PluginsConfig => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PluginsConfig"].ToString());

	public static string PluginsPath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PluginsPath"].ToString());

	public static string PluginsIconFolderPath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PluginsIconFolderPath"].ToString());

	public static string AplicationDocPath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ApplicationDoc"].ToString());

	public static AbstractDataBase PluginDataBase
	{
		get
		{
			if (m_xmlPluginData == null)
			{
				m_xmlPluginData = new XmlPluginData(PluginsConfig, PluginsPath);
			}
			return m_xmlPluginData;
		}
	}

	public static string DownloadPath
	{
		get
		{
			string text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["DownloadPath"].ToString());
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public static string ToolNewVersionConfig => ConfigurationManager.AppSettings["ToolNewVersionConfig"].ToString();

	public static string PluginNewVersionConfig => ConfigurationManager.AppSettings["PluginNewVersionConfig"].ToString();

	public static string VersionDownloadUrlHeader => ConfigurationManager.AppSettings["DownloadUrlHeader"].ToString();

	public static Ellipse NewVersionTip { get; set; }

	public static bool FrameHasNewVersion
	{
		get
		{
			return frameHasNewVersion;
		}
		set
		{
			frameHasNewVersion = value;
			if (NewVersionTip != null)
			{
				NewVersionTip.Visibility = ((!frameHasNewVersion) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
	}

	public static Plugin LastSelectedPlugin
	{
		get
		{
			return lastSelectedPlugin;
		}
		set
		{
			lastSelectedPlugin = value;
		}
	}

	public static UpdateTipWindow UpdateTip
	{
		get
		{
			return updateTip;
		}
		set
		{
			updateTip = value;
		}
	}

	public static PlugInPanelModel PlugInPanelModelCurrentSelected
	{
		get
		{
			return plugInPanelModelCurrentSelected;
		}
		set
		{
			plugInPanelModelCurrentSelected = value;
		}
	}

	public static PlugInPanelModel PlugInPanelModelCurrentUpdated
	{
		get
		{
			return plugInPanelModelCurrentUpdated;
		}
		set
		{
			plugInPanelModelCurrentUpdated = value;
		}
	}

	public static bool IsUpdatingPlug
	{
		get
		{
			return isUpdatingPlug;
		}
		set
		{
			isUpdatingPlug = value;
		}
	}

	public static PluginVersionModel NewPluginModel { get; set; }

	public static List<PluginVersionModel> m_haveNewVesrionList { get; set; }

	private ApplcationClass()
	{
	}

	private static void CreateScreenView(string pluginName, string pluginVersion)
	{
		string empty = string.Empty;
		empty = pluginName.ToLower() switch
		{
			"rescue" => s_PluginFlashScreenName, 
			"my device" => s_PluginDeviceScreenName, 
			"support" => s_PluginSupportScreenName, 
			"toolbox" => s_PluginToolboxScreenName, 
			"forum" => s_PluginForumScreenName, 
			"tips" => s_PluginTipsScreenName, 
			"chat" => s_PluginMessengerScreenName, 
			_ => s_PluginDefaultScreenName, 
		};
		if (!string.IsNullOrEmpty(empty))
		{
			global::Smart.GoogleAnalyticsTracker.Tracker.ScreenName = empty;
			global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateScreenView().Set("av", pluginVersion).Build());
		}
	}

	public static ApplcationClass CreateInstance()
	{
		if (_instance == null)
		{
			lock (lockHelper)
			{
				if (_instance == null)
				{
					_instance = new ApplcationClass();
				}
			}
		}
		return _instance;
	}

	public static void ShowCrashPanel(UIElement ui)
	{
	}

	public static void HideCrashPanel()
	{
	}

	public static void SetContentPresenterPlugin(UIElement uielement)
	{
		if (ContentPresenterPlugin != null)
		{
			ContentPresenterPlugin.Content = null;
			ContentPresenterPlugin.Content = uielement;
		}
	}

	public static void AddFreamElementToUI(object data)
	{
		LogHelper.LogInstance.Info("start plugin:" + PlugInPanelModelCurrentSelected.TargetPluginInfo.PluginName);
		PluginInfo targetPluginInfo = PlugInPanelModelCurrentSelected.TargetPluginInfo;
		MainWindowControl.Instance.LoadPluginCallback(delegate(Plugin plugin)
		{
			try
			{
				if (!isAddPlugIning)
				{
					isAddPlugIning = true;
					if (plugin != null)
					{
						CurrentPlugin = plugin.PluginInstance;
						HostProxy.HostNavigation.CurrentPluginID = plugin.PluginInfo.PluginID;
						plugin.PluginInstance.OnInit(data);
						FrameworkElement view = plugin.View;
						LastSelectedPlugin = plugin;
						if (view != null)
						{
							SetContentPresenterPlugin(view);
							CreateScreenView(plugin.PluginInfo.PluginName, plugin.PluginInfo.Version);
						}
					}
					else
					{
						CurrentPlugin = null;
						MessageBox_Common messageBox_Common = new MessageBox_Common(ApplcationStartWindow, TypeItems.MessageBoxType.OK, "K0347", "K0327", "");
						LogHelper.LogInstance.Error($"start plugin: {PlugInPanelModelCurrentSelected.TargetPluginInfo.PluginName} failed!");
						messageBox_Common.ShowDialog();
						PluginAddFailed contentPresenterPlugin = new PluginAddFailed();
						SetContentPresenterPlugin(contentPresenterPlugin);
					}
					PlugInPanel.listBoxMain.IsEnabled = true;
				}
			}
			finally
			{
				isAddPlugIning = false;
			}
		}, targetPluginInfo);
	}
}
