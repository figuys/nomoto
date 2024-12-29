using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class StartViewModel : ViewModelBase
{
	private string _ViewName = ViewType.START.ToString();

	public ReplayCommand GotoWifiCommand { get; }

	public ReplayCommand GotoUsbCommand { get; }

	public ReplayCommand WifiHelperCommand { get; }

	public ReplayCommand TutorialsCommand { get; }

	public string ViewName
	{
		get
		{
			return _ViewName;
		}
		set
		{
			_ViewName = value;
			OnPropertyChanged("ViewName");
		}
	}

	public MainFrameViewModel MainFrame => Context.MainFrame;

	public StartViewModel()
	{
		GotoWifiCommand = new ReplayCommand(GotoWifiCommandHandler);
		GotoUsbCommand = new ReplayCommand(GotoUsbCommandHandler);
		WifiHelperCommand = new ReplayCommand(WifiHelperCommandHandler);
		TutorialsCommand = new ReplayCommand(TutorialsCommandHandler);
	}

	public override void LoadData(object data)
	{
		IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
		if (conntectedDevices != null && conntectedDevices.Count((DeviceEx n) => n.ConnectType != ConnectType.Fastboot && n.SoftStatus == DeviceSoftStateEx.Connecting) > 0)
		{
			ViewName = ViewType.START_CONNECTING.ToString();
		}
		else
		{
			ViewName = data.ToString();
		}
		base.LoadData(data);
	}

	private void GotoWifiCommandHandler(object data)
	{
		Context.Switch(ViewType.START_WIFI, ViewType.START_WIFI, reload: false, reloadData: true);
	}

	private void GotoUsbCommandHandler(object data)
	{
		Context.Switch(ViewType.START, ViewType.START, reload: false, reloadData: true);
	}

	private void WifiHelperCommandHandler(object data)
	{
		IUserMsgControl userUi = new WifiConnectHelpWindowV6
		{
			DataContext = new WifiConnectHelpWindowModelV6()
		};
		Context.MessageBox.ShowMessage(userUi);
	}

	private void TutorialsCommandHandler(object data)
	{
		Context.Switch(ViewType.START_TUTORIALS, ViewType.START_TUTORIALS, reload: false, reloadData: true);
	}
}
