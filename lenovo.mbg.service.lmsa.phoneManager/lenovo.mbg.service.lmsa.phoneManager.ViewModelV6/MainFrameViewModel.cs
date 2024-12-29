using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ModelV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class MainFrameViewModel : ViewModelBase
{
	public static MainFrameViewModel Instance = new MainFrameViewModel();

	public ObservableCollection<ConnectedDeviceViewModel> _ConnectedDevices = new ObservableCollection<ConnectedDeviceViewModel>();

	private Visibility _NavVisibility = Visibility.Collapsed;

	private LeftNavigationModelV6 _SelectedNav;

	private object _CurrentView;

	private string _Content = "K1375";

	private Visibility _RefreshVisibile = Visibility.Collapsed;

	private double _ConnectPer;

	private ImageSource _qrImageSource;

	public ReplayCommand LeftNavigationSelectionChanged { get; private set; }

	public ObservableCollection<ConnectedDeviceViewModel> ConnectedDevices
	{
		get
		{
			return _ConnectedDevices;
		}
		set
		{
			_ConnectedDevices = value;
			OnPropertyChanged("ConnectedDevices");
		}
	}

	public Visibility NavVisibility
	{
		get
		{
			return _NavVisibility;
		}
		set
		{
			_NavVisibility = value;
			OnPropertyChanged("NavVisibility");
		}
	}

	public LeftNavigationModelV6 SelectedNav
	{
		get
		{
			return _SelectedNav;
		}
		set
		{
			_SelectedNav = value;
			if (_SelectedNav == null)
			{
				Context.Switch(ViewType.START, ViewType.START, reload: false, reloadData: true);
				ChangeNavVisibility(Visibility.Collapsed);
			}
			else
			{
				bool flag = _SelectedNav.Reload;
				if (flag)
				{
					_SelectedNav.Reload = false;
				}
				ViewType viewType = _SelectedNav.Key;
				if (viewType == ViewType.ONEKEYCLONE && Context.OneKyCloneSubViewType != viewType)
				{
					viewType = Context.OneKyCloneSubViewType;
				}
				if (viewType == ViewType.APP)
				{
					flag = true;
				}
				Task.Run(delegate
				{
					if (Context.ViewType2BusinessType.ContainsKey(viewType))
					{
						BusinessType businessType = Context.ViewType2BusinessType[viewType];
						HostProxy.BehaviorService.Collect(businessType, new BusinessData(businessType, Context.CurrentDevice));
					}
				});
				Context.Switch(viewType, Context.CurrentDevice, reload: false, flag);
				ChangeNavVisibility(Visibility.Visible);
			}
			OnPropertyChanged("SelectedNav");
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

	public ReplayCommand RefreshCommand { get; }

	public string Content
	{
		get
		{
			return _Content;
		}
		set
		{
			_Content = value;
			OnPropertyChanged("Content");
		}
	}

	public Visibility RefreshVisibile
	{
		get
		{
			return _RefreshVisibile;
		}
		set
		{
			_RefreshVisibile = value;
			OnPropertyChanged("RefreshVisibile");
		}
	}

	public double ConnectPer
	{
		get
		{
			return _ConnectPer;
		}
		set
		{
			_ConnectPer = value;
			OnPropertyChanged("ConnectPer");
		}
	}

	public ObservableCollection<LeftNavigationModelV6> LeftNavigationItems { get; set; }

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

	private MainFrameViewModel()
	{
		Context.CurrentViewType = ViewType.START;
		ConnectedDevices = new ObservableCollection<ConnectedDeviceViewModel>();
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		InitNav();
		LeftNavigationSelectionChanged = new ReplayCommand(LeftNavigationSelectionChangedHandler);
	}

	public override void LoadData()
	{
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice == null || masterDevice.SoftStatus != DeviceSoftStateEx.Online)
		{
			Context.Switch(ViewType.START, ViewType.START, reload: false, reloadData: true);
			ChangeNavVisibility(Visibility.Collapsed);
		}
		HostProxy.deviceManager.Connecte += DeviceManager_Connecte;
		HostProxy.deviceManager.DisConnecte += DeviceManager_DisConnecte;
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_OnMasterDeviceChanged;
		HostProxy.deviceManager.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
		OnWifiMonitoringEndPointChanged(HostProxy.deviceManager.WirelessWaitForConnectEndPoints);
		base.LoadData();
	}

	private void InitNav()
	{
		LeftNavigationItems = new ObservableCollection<LeftNavigationModelV6>();
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.HOME, "K1737", "icon_home_unactived", "icon_home_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.APP, "K0474", "icon_apps_unactived", "icon_apps_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.PICTURE, "K0475", "icon_pictures_unactived", "icon_pictures_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.MUSIC, "K0476", "icon_music_unactived", "icon_music_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.VIDEO, "K0477", "icon_videos_unactived", "icon_videos_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.CONTACT, "K0478", "icon_contacts_unactived", "icon_contacts_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.FILE, "K0480", "icon_file_unactived", "icon_file_actived"));
		LeftNavigationItems.Add(new LeftNavigationModelV6(ViewType.ONEKEYCLONE, "K0596", "icon_clone_unactived", "icon_clone_actived"));
	}

	public void ChangeNavEnabled(bool isEnabled)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			foreach (LeftNavigationModelV6 leftNavigationItem in LeftNavigationItems)
			{
				if (leftNavigationItem.Key != ViewType.ONEKEYCLONE)
				{
					leftNavigationItem.IsEnable = isEnabled;
				}
			}
		});
	}

	private void DeviceManager_Connecte(object sender, DeviceEx e)
	{
		if (e.ConnectType == ConnectType.Adb || e.ConnectType == ConnectType.Wifi)
		{
			e.SoftStatusChanged += FireConnectSoftStatusChanged;
		}
	}

	private void DeviceManager_DisConnecte(object sender, DeviceEx e)
	{
		e.SoftStatusChanged -= FireConnectSoftStatusChanged;
		LogHelper.LogInstance.Info("TestLog1: discount remove device start");
		RemoveDevice(e);
	}

	private void DeviceManager_OnMasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		foreach (LeftNavigationModelV6 leftNavigationItem in LeftNavigationItems)
		{
			leftNavigationItem.Reload = true;
		}
		if (e.Previous != null)
		{
			TcpAndroidDevice obj = e.Previous as TcpAndroidDevice;
			obj.SoftStatusChanged -= FireMasterSoftStatusChanged;
			obj.TcpConnectStepChanged -= FireTcpConnectStepChanged;
		}
		if (e.Current != null)
		{
			TcpAndroidDevice obj2 = e.Current as TcpAndroidDevice;
			obj2.SoftStatusChanged += FireMasterSoftStatusChanged;
			obj2.TcpConnectStepChanged += FireTcpConnectStepChanged;
		}
	}

	private void FireMasterSoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		DeviceEx deviceEx = (DeviceEx)sender;
		switch (e)
		{
		case DeviceSoftStateEx.Online:
			Context.CurrentDevice = deviceEx;
			if (Convert.ToInt32(Context.CurrentViewType) < 80)
			{
				SelectedNav = LeftNavigationItems.First();
			}
			break;
		case DeviceSoftStateEx.Offline:
		case DeviceSoftStateEx.ManualDisconnect:
			LogHelper.LogInstance.Info("TestLog: " + deviceEx.Identifer + " offline");
			Context.CurrentDevice = null;
			RemoveDevice(deviceEx);
			break;
		case DeviceSoftStateEx.Connecting:
			Context.Switch(ViewType.START_CONNECTING, ViewType.START_CONNECTING, reload: false, reloadData: true);
			break;
		case DeviceSoftStateEx.Reconncting:
			break;
		}
	}

	private void FireConnectSoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		TcpAndroidDevice d = sender as TcpAndroidDevice;
		ConnectedDeviceViewModel connectedDeviceViewModel = ConnectedDevices.FirstOrDefault((ConnectedDeviceViewModel n) => n.Id == d.Identifer);
		if (e == DeviceSoftStateEx.Online && connectedDeviceViewModel == null)
		{
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				ConnectedDevices.Add(new ConnectedDeviceViewModel(d));
			});
		}
	}

	private void RemoveDevice(DeviceEx device)
	{
		ConnectedDeviceViewModel found = ConnectedDevices.FirstOrDefault((ConnectedDeviceViewModel n) => n.Id == device.Identifer);
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			LogHelper.LogInstance.Info("TestLog: " + found?.Device?.Identifer + " will be remove");
			ConnectedDevices.Remove(found);
			if (ConnectedDevices.Count == 0)
			{
				LogHelper.LogInstance.Info("TestLog: will show start page");
				SelectedNav = null;
			}
		});
	}

	private void FireTcpConnectStepChanged(object sender, TcpConnectStepChangedEventArgs e)
	{
		string content = string.Empty;
		Application.Current.Dispatcher.Invoke(delegate
		{
			if (e.Result == ConnectStepStatus.Fail)
			{
				RefreshVisibile = Visibility.Visible;
			}
			else
			{
				RefreshVisibile = Visibility.Collapsed;
			}
		});
		switch (e.Step)
		{
		case "AppVersionIsMatched":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				content = "K0765";
				break;
			case ConnectStepStatus.Starting:
				content = "K0758";
				break;
			case ConnectStepStatus.Success:
				content = "K0766";
				break;
			}
			break;
		case "UnInstallApp":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				content = "K1379";
				break;
			case ConnectStepStatus.Starting:
				content = "K0775";
				break;
			case ConnectStepStatus.Success:
				content = "K0776";
				break;
			}
			break;
		case "InstallApp":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				content = "K1380";
				break;
			case ConnectStepStatus.Starting:
				content = "K0759";
				break;
			case ConnectStepStatus.Success:
				content = "K0760";
				break;
			}
			break;
		case "TcpConnect":
		{
			TcpAndroidDevice tcpAndroidDevice = sender as TcpAndroidDevice;
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				content = ((!"Moto".Equals(tcpAndroidDevice?.ConnectedAppType)) ? "K0768" : ((e.ErrorCode != ConnectErrorCode.TcpConnectFailWithAppNotAllowed) ? "K0792" : "K0801"));
				break;
			case ConnectStepStatus.Starting:
				content = ((!"Moto".Equals(tcpAndroidDevice?.ConnectedAppType)) ? "K0738" : "K0789");
				break;
			case ConnectStepStatus.Success:
				content = "K0761";
				break;
			}
			break;
		}
		case "LoadDeviceProperty":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				content = "K1381";
				break;
			case ConnectStepStatus.Starting:
				content = "K0445";
				break;
			case ConnectStepStatus.Success:
				content = "K0762";
				break;
			}
			break;
		}
		Content = content;
		ConnectPer = e.Percent;
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

	private void RefreshCommandHandler(object data)
	{
		if (HostProxy.deviceManager.MasterDevice == null)
		{
			Context.Switch(ViewType.START, ViewType.START, reload: false);
			ChangeNavVisibility(Visibility.Collapsed);
		}
		else
		{
			HostProxy.deviceManager.MasterDevice.PhysicalStatus = DevicePhysicalStateEx.Offline;
		}
	}

	private void LeftNavigationSelectionChangedHandler(object parameter)
	{
		Context.Switch(((LeftNavigationModelV6)parameter).Key);
	}

	private void ChangeNavVisibility(Visibility visibile)
	{
		HostProxy.CurrentDispatcher?.Invoke(() => NavVisibility = visibile);
	}
}
