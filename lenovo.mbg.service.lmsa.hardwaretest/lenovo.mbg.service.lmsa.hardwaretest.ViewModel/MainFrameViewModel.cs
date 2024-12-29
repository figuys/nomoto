using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.hardwaretest.ViewModel;

public class MainFrameViewModel : ViewModelBase
{
	private object _CurrentView;

	private Visibility _BottomLineVisibility = Visibility.Collapsed;

	private ImageSource _qrImageSource;

	public DeviceEx CurrentDevice
	{
		get
		{
			return Context.CurrentDevice;
		}
		private set
		{
			Context.CurrentDevice = value;
			if (value == null)
			{
				Context.Dispatcher.Invoke(delegate
				{
					BottomLineVisibility = Visibility.Collapsed;
					Context.Switch(ViewType.START);
				});
			}
			else
			{
				Context.Dispatcher.Invoke(delegate
				{
					BottomLineVisibility = Visibility.Visible;
					Context.Switch(ViewType.MAIN, reloadData: true);
				});
			}
		}
	}

	public object CurrentView
	{
		get
		{
			return _CurrentView;
		}
		set
		{
			_CurrentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public Visibility BottomLineVisibility
	{
		get
		{
			return _BottomLineVisibility;
		}
		set
		{
			_BottomLineVisibility = value;
			OnPropertyChanged("BottomLineVisibility");
		}
	}

	public ImageSource QrImageSource
	{
		get
		{
			return _qrImageSource;
		}
		set
		{
			_qrImageSource = value;
			OnPropertyChanged("QrImageSource");
		}
	}

	public override void LoadData()
	{
		Context.Switch(ViewType.START);
		HostProxy.deviceManager.MasterDeviceChanged += FireMasterDeviceChanged;
		HostProxy.deviceManager.Connecte += delegate(object sender, DeviceEx e)
		{
			e.SoftStatusChanged += OnSoftStatusChanged;
		};
		HostProxy.deviceManager.DisConnecte += delegate(object sender, DeviceEx e)
		{
			e.SoftStatusChanged -= OnSoftStatusChanged;
		};
		HostProxy.deviceManager.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
		OnWifiMonitoringEndPointChanged(HostProxy.deviceManager.WirelessWaitForConnectEndPoints);
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice != null && !masterDevice.IsHWEnable())
		{
			DeviceEx deviceEx = HostProxy.deviceManager.ConntectedDevices.FirstOrDefault((DeviceEx p) => p.IsHWEnable());
			if (deviceEx != null)
			{
				HostProxy.deviceManager.SwitchDevice(deviceEx.Identifer);
			}
		}
		base.LoadData();
	}

	private void OnSoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		if (HostProxy.HostNavigation.CurrentPluginID != "985c66acdde2483ed96844a6b5ea4337")
		{
			return;
		}
		DeviceEx deviceEx = sender as DeviceEx;
		if (deviceEx.SoftStatus == DeviceSoftStateEx.Online)
		{
			if (deviceEx.ConnectType == ConnectType.Wifi)
			{
				GlobalCmdHelper.Instance.Execute(new GlobalCmdData
				{
					type = GlobalCmdType.CLOSE_WIFI_TUTORIAL
				});
			}
			DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
			if (masterDevice != null && !masterDevice.IsHWEnable() && deviceEx.IsHWEnable())
			{
				HostProxy.deviceManager.SwitchDevice(deviceEx.Identifer);
			}
		}
	}

	private void FireMasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged -= SoftStatusChangedHandler;
			e.Current.SoftStatusChanged += SoftStatusChangedHandler;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= SoftStatusChangedHandler;
		}
	}

	private void SoftStatusChangedHandler(object sender, DeviceSoftStateEx e)
	{
		DeviceEx deviceEx = (DeviceEx)sender;
		if (e == DeviceSoftStateEx.Online)
		{
			if (deviceEx.ConnectType == ConnectType.Wifi)
			{
				GlobalCmdHelper.Instance.Execute(new GlobalCmdData
				{
					type = GlobalCmdType.CLOSE_WIFI_TUTORIAL
				});
			}
			SetHWTestDevice(deviceEx);
		}
		else
		{
			CurrentDevice = null;
		}
	}

	public void SetHWTestDevice(DeviceEx device)
	{
		if (HostProxy.HostNavigation.CurrentPluginID != "985c66acdde2483ed96844a6b5ea4337")
		{
			return;
		}
		if (device.IsHWEnable())
		{
			CurrentDevice = device;
			return;
		}
		DeviceEx deviceEx = HostProxy.deviceManager.ConntectedDevices.FirstOrDefault((DeviceEx p) => p.IsHWEnable());
		if (deviceEx != null)
		{
			HostProxy.deviceManager.SwitchDevice(deviceEx.Identifer);
		}
		else
		{
			MotoHelperCheck(device);
		}
	}

	private void MotoHelperCheck(DeviceEx device)
	{
		if (Context.MessageBox.ShowMessage("K0071", "K1475", "K0327", "K1444", isCloseBtn: false, null, MessageBoxImage.Exclamation) == true)
		{
			if (device is AdbDeviceEx adbDeviceEx)
			{
				adbDeviceEx.ForceInstallMa();
			}
			Context.Dispatcher.Invoke(delegate
			{
				IUserMsgControl userUi = new WifiConnectHelpWindowV6(isHwPop: true, string.Empty, WifiTutorialsType.HWTEST)
				{
					DataContext = new WifiConnectHelpWindowModelV6(WifiTutorialsType.HWTEST)
				};
				Context.MessageBox.ShowMessage(userUi);
			});
		}
	}

	private void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> underWatchEndPoints)
	{
		if (underWatchEndPoints == null)
		{
			return;
		}
		StringBuilder sb = new StringBuilder();
		sb.Append($"V:{Configurations.AppVersionCode}").Append(Environment.NewLine);
		sb.Append($"DV:{Configurations.AppMinVersionCodeOfMoto}").Append(Environment.NewLine);
		foreach (Tuple<string, string> underWatchEndPoint in underWatchEndPoints)
		{
			sb.Append("IP:").Append(underWatchEndPoint.Item1).Append(Environment.NewLine);
		}
		try
		{
			HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
			{
				MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(sb.ToString());
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
				bitmapImage.EndInit();
				try
				{
					QrImageSource = bitmapImage;
				}
				catch (Exception)
				{
				}
			});
		}
		catch (Exception)
		{
		}
	}
}
