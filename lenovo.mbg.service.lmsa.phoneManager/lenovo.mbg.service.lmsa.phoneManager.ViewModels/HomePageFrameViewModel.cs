using System;
using System.Collections.Generic;
using System.Windows;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class HomePageFrameViewModel : ViewModelBase
{
	private static HomePageFrameViewModel single;

	private FrameworkElement currrentView;

	public static HomePageFrameViewModel Single => single ?? (single = new HomePageFrameViewModel());

	public Dictionary<Type, FrameworkElement> ViewsMapping { get; set; }

	public FrameworkElement CurrentView
	{
		get
		{
			return currrentView;
		}
		set
		{
			if (currrentView != value)
			{
				currrentView = value;
				OnPropertyChanged("CurrentView");
			}
		}
	}

	private HomePageFrameViewModel()
	{
		ViewsMapping = new Dictionary<Type, FrameworkElement>();
		ViewsMapping.Add(typeof(HomeView), new HomeView
		{
			DataContext = HomeViewModel.SingleInstance
		});
		ViewsMapping.Add(typeof(DisConnectedViewNew), new DisConnectedViewNew
		{
			DataContext = new DisConnectedViewModel()
		});
		ViewsMapping.Add(typeof(USBConnectHelper), new USBConnectHelper
		{
			DataContext = new USBConnectHelperViewModel()
		});
		ViewsMapping.Add(typeof(USBConnectingFailView), new USBConnectingFailView
		{
			DataContext = new USBConnectingFailViewModel()
		});
		ViewsMapping.Add(typeof(WifiConnection), new WifiConnection
		{
			DataContext = new WifiConnectionViewModel()
		});
		ViewsMapping.Add(typeof(WifiConnectingFailView), new WifiConnectingFailView
		{
			DataContext = new WifiConnectingFailViewModel()
		});
		ViewsMapping.Add(typeof(ConnectingView), new ConnectingView
		{
			DataContext = new ConnectingViewModel()
		});
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
		_ = HostProxy.deviceManager.MasterDevice;
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		else
		{
			DeviceSoftStateEx e2 = ((e?.Previous?.SoftStatus == DeviceSoftStateEx.ManualDisconnect) ? DeviceSoftStateEx.ManualDisconnect : DeviceSoftStateEx.Offline);
			Current_SoftStatusChanged(e.Previous, e2);
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		switch (e)
		{
		case DeviceSoftStateEx.ManualDisconnect:
			Switch(typeof(WifiConnection));
			break;
		case DeviceSoftStateEx.Offline:
		{
			DeviceEx deviceEx = sender as DeviceEx;
			ConnectType? connectType = deviceEx?.ConnectType;
			if (ConnectType.Wifi == connectType)
			{
				Switch(typeof(WifiConnectingFailView));
			}
			else if (ConnectType.Adb == connectType && deviceEx.PhysicalStatus != 0)
			{
				Switch(typeof(USBConnectingFailView));
			}
			else
			{
				Switch(typeof(DisConnectedViewNew));
			}
			break;
		}
		case DeviceSoftStateEx.Connecting:
			Switch(typeof(ConnectingView));
			break;
		case DeviceSoftStateEx.Online:
			Switch(typeof(HomeView));
			break;
		default:
			Switch(typeof(DisConnectedViewNew));
			break;
		case DeviceSoftStateEx.Reconncting:
			break;
		}
	}

	public void Switch(Type viewType)
	{
		foreach (KeyValuePair<Type, FrameworkElement> item in ViewsMapping)
		{
			if (item.Key.Equals(viewType))
			{
				CurrentView = item.Value;
				break;
			}
		}
	}
}
