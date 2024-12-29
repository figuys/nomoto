using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GoogleAnalytics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.ViewV6;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.AttachedProperty;

namespace lenovo.mbg.service.lmsa.flash;

[PluginExport(typeof(IPlugin), "8ab04aa975e34f1ca4f9dc3a81374e2c")]
public class Plugin : PluginBase
{
	public static Tracker Tracker => HostProxy.GoogleAnalyticsTracker.Tracker;

	public static bool IsRescuePlugin => HostProxy.HostNavigation.CurrentPluginID == "8ab04aa975e34f1ca4f9dc3a81374e2c";

	public static bool IsExecuteRescueWork { get; set; }

	public static bool SupportMulti { get; set; }

	public static IMessageBox IMsgManager { get; private set; }

	public static void OperateTracker(string operate, string description)
	{
		LogHelper.LogInstance.Info(description);
		try
		{
			Tracker.Send(HitBuilder.CreateCustomEvent("lmsa-plugin-flash", operate, description, 0L).Build());
		}
		catch
		{
		}
	}

	public static void ShowMutilIcon()
	{
		IMsgManager.ShowMutilIcon(MainFrameV6.Instance.CurrentPageIndex == PageIndex.RESCUE_FLASH, showList: false);
	}

	public override void Init()
	{
		SupportMulti = HostProxy.User.user.IsB2BSupportMultDev;
		Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
		{
			Source = new Uri("/lenovo.mbg.service.lmsa.flash;component/UserResources/Style.xaml", UriKind.Relative)
		});
		Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
		{
			Source = new Uri("/lenovo.mbg.service.lmsa.flash;component/Themes/Generic.xaml", UriKind.Relative)
		});
		FlashContext.SingleInstance.Initialize();
	}

	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		Adorners.CurrentDispatcher = Dispatcher.CurrentDispatcher;
		IMsgManager = iMessage;
		return new MainFrameV6();
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override bool CanClose()
	{
		return false;
	}

	public override bool IsExecuteWork()
	{
		return IsExecuteRescueWork;
	}

	public override void OnInit(object data)
	{
		base.OnInit(data);
		ShowMutilIcon();
		if (data is Dictionary<string, string>)
		{
			Dictionary<string, string> dictionary = data as Dictionary<string, string>;
			LogHelper.LogInstance.Info(string.Format("Rescue plugin init by Download center resource, category is {0}", dictionary?.ContainsKey("category")));
			PageIndex index = PageIndex.PHONE_MANUAL;
			if (dictionary.ContainsKey("category"))
			{
				if (dictionary["category"] == $"{DevCategory.Tablet}")
				{
					index = PageIndex.TABLET_MANUAL;
				}
				else if (dictionary["category"] == $"{DevCategory.Smart}")
				{
					index = PageIndex.SMART_MANUAL;
				}
			}
			MainFrameV6.Instance.ChangeView(index, dictionary);
		}
		else if (data is string text && text == "mydevicerescue")
		{
			LogHelper.LogInstance.Info("match device when switch to Rescue plugin");
			Task.Run(delegate
			{
				MainFrameV6.Instance.AutoMatch(HostProxy.deviceManager.MasterDevice, jumpToMatchView: true);
			});
		}
	}
}
