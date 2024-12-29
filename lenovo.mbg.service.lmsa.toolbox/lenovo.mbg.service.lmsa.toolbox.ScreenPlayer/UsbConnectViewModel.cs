using System;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Dialog;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class UsbConnectViewModel : NotifyBase
{
	private bool _IsBtnEnable;

	private Visibility _DeviceLoadingVisible;

	public ReplayCommand NextCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public ReplayCommand TutorialCommand { get; set; }

	public ReplayCommand DownloadCommand { get; set; }

	public bool IsOk { get; set; }

	public int CloseMode { get; set; }

	public bool IsBtnEnable
	{
		get
		{
			return _IsBtnEnable;
		}
		set
		{
			_IsBtnEnable = value;
			OnPropertyChanged("IsBtnEnable");
		}
	}

	public Visibility DeviceLoadingVisible
	{
		get
		{
			return _DeviceLoadingVisible;
		}
		set
		{
			_DeviceLoadingVisible = value;
			OnPropertyChanged("DeviceLoadingVisible");
		}
	}

	public UsbConnectViewModel(Window wnd)
	{
		UsbConnectViewModel usbConnectViewModel = this;
		DeviceLoadingVisible = Visibility.Collapsed;
		NextCommand = new ReplayCommand(delegate(object param)
		{
			usbConnectViewModel.IsOk = true;
			usbConnectViewModel.CloseMode = (Convert.ToBoolean(param) ? 1 : 2);
			if (HostProxy.deviceManager.MasterDevice != null)
			{
				HostProxy.deviceManager.MasterDevice.SoftStatusChanged -= usbConnectViewModel.OnMasterDeviceOnLine;
			}
			HostProxy.deviceManager.MasterDeviceChanged -= usbConnectViewModel.OnMasterDeviceChanged;
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				wnd.Close();
			});
		});
		CloseCommand = new ReplayCommand(delegate
		{
			usbConnectViewModel.IsOk = false;
			usbConnectViewModel.CloseMode = 0;
			if (HostProxy.deviceManager.MasterDevice != null)
			{
				HostProxy.deviceManager.MasterDevice.SoftStatusChanged -= usbConnectViewModel.OnMasterDeviceOnLine;
			}
			HostProxy.deviceManager.MasterDeviceChanged -= usbConnectViewModel.OnMasterDeviceChanged;
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				wnd.Close();
			});
		});
		TutorialCommand = new ReplayCommand(delegate
		{
			UsbDebugTutorialView view = new UsbDebugTutorialView();
			HostProxy.HostMaskLayerWrapper.New(view, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => view.ShowDialog());
		});
		DownloadCommand = new ReplayCommand(delegate
		{
			GlobalFun.OpenUrlByBrowser(HostProxy.LanguageService.IsChinaRegionAndLanguage() ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
		});
		IsBtnEnable = false;
		if (HostProxy.deviceManager.MasterDevice != null)
		{
			HostProxy.deviceManager.MasterDevice.SoftStatusChanged += OnMasterDeviceOnLine;
		}
		HostProxy.deviceManager.MasterDeviceChanged += OnMasterDeviceChanged;
	}

	private void OnMasterDeviceOnLine(object sender, DeviceSoftStateEx e)
	{
		if (e == DeviceSoftStateEx.Online)
		{
			DeviceLoadingVisible = Visibility.Collapsed;
			NextCommand.Execute(true);
		}
	}

	private void OnMasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			DeviceLoadingVisible = Visibility.Visible;
			e.Current.SoftStatusChanged += OnMasterDeviceOnLine;
		}
		if (e.Previous != null)
		{
			if (e.Current == null)
			{
				DeviceLoadingVisible = Visibility.Collapsed;
			}
			e.Previous.SoftStatusChanged -= OnMasterDeviceOnLine;
		}
	}
}
