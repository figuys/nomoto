using System;
using System.IO;
using System.Windows;
using GoogleAnalytics;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.toolbox;

[PluginExport(typeof(IPlugin), "dd537b5c6c074ae49cc8b0b2965ce54a")]
public class Plugin : PluginBase
{
	private ToolboxFrameV6 _toolBoxWindow;

	private string abdPath = string.Empty;

	public const string toolboxScreenName = "lmsa-plugin-Toolbox-main";

	public const string clipboardScreenName = "lmsa-plugin-toolbox-clipboard";

	public const string gifMakerScreenName = "lmsa-plugin-toolbox-gifmaker";

	public const string ringtoneMakerScreenName = "lmsa-plugin-toolbox-ringtonemaker";

	public const string screenCaptureScreenName = "lmsa-plugin-toolbox-screencapture";

	public static readonly string Category = "Plugin-Toolbox";

	public static Tracker Tracker => HostProxy.GoogleAnalyticsTracker.Tracker;

	public override void Init()
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		resourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.toolbox;component/Themes/AllResourceDir.xaml", UriKind.Relative);
		Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
		ToolboxViewContext.SingleInstance.Initialize();
	}

	public override FrameworkElement CreateControl(IMessageBox iMsg)
	{
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		_toolBoxWindow = new ToolboxFrameV6();
		ToolboxViewContext.SingleInstance.MessageBox = iMsg;
		return _toolBoxWindow;
	}

	public override void Dispose()
	{
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
		if (Directory.Exists(path))
		{
			Directory.Delete(path, recursive: true);
		}
		base.Dispose();
	}

	public override bool CanClose()
	{
		return false;
	}

	public override bool IsExecuteWork()
	{
		return false;
	}
}
