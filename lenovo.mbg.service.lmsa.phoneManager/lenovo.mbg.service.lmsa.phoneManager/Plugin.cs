using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;

namespace lenovo.mbg.service.lmsa.phoneManager;

[PluginExport(typeof(IPlugin), "02928af025384c75ae055aa2d4f256c8")]
public class Plugin : PluginBase
{
	public const string PLUGIN_ID = "02928af025384c75ae055aa2d4f256c8";

	public override void Init()
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		resourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Themes/Themes.xaml", UriKind.Relative);
		Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
		Application.Current.Resources.MergedDictionaries.Add(BRComponentResources.SingleInstance.ResourceDictionary);
	}

	public override FrameworkElement CreateControl(IMessageBox iMsg)
	{
		MainFrameV6 result = new MainFrameV6
		{
			DataContext = Context.MainFrame
		};
		Context.MainFrame.LoadData();
		Context.MessageBox = iMsg;
		return result;
	}

	public override bool IsExecuteWork()
	{
		return Context.IsExecuteWork;
	}

	public override void OnInit(object data)
	{
		base.OnInit(data);
		IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
		if (conntectedDevices == null || conntectedDevices.Count((DeviceEx n) => n.ConnectType != ConnectType.Fastboot && n.SoftStatus == DeviceSoftStateEx.Online) == 0)
		{
			Context.MainFrame.SelectedNav = null;
		}
	}
}
