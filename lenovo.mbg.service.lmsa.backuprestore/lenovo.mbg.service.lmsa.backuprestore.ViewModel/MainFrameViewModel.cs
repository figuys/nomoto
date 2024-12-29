using System.Windows;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class MainFrameViewModel : ViewModelBase
{
	private object _currentView;

	private Visibility _MatchVisibility = Visibility.Hidden;

	private Visibility _MatchLoadingVisibility;

	private Visibility _BackVisibility = Visibility.Collapsed;

	private Visibility _BottomLineVisibility = Visibility.Collapsed;

	private const string connectFailed = "K0768";

	private const string connecting = "K0738";

	private string _matchText = "K0738";

	protected DeviceEx CurrentDevice
	{
		get
		{
			return Context.CurrentDevice;
		}
		set
		{
			if (value == null && Context.CurrentDevice != null)
			{
				Context.CurrentDevice.SoftStatusChanged -= Current_SoftStatusChanged;
			}
			Context.CurrentDevice = value;
		}
	}

	public ReplayCommand BackCommand { get; }

	public object CurrentView
	{
		get
		{
			return _currentView;
		}
		set
		{
			_currentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public Visibility MatchVisibility
	{
		get
		{
			return _MatchVisibility;
		}
		set
		{
			_MatchVisibility = value;
			OnPropertyChanged("MatchVisibility");
		}
	}

	public Visibility MatchLoadingVisibility
	{
		get
		{
			return _MatchLoadingVisibility;
		}
		set
		{
			_MatchLoadingVisibility = value;
			OnPropertyChanged("MatchLoadingVisibility");
		}
	}

	public Visibility BackVisibility
	{
		get
		{
			return _BackVisibility;
		}
		set
		{
			_BackVisibility = value;
			OnPropertyChanged("BackVisibility");
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

	public string MatchText
	{
		get
		{
			return _matchText;
		}
		set
		{
			_matchText = value;
			if (_matchText == "K0768")
			{
				MatchLoadingVisibility = Visibility.Collapsed;
			}
			else
			{
				MatchLoadingVisibility = Visibility.Visible;
			}
			OnPropertyChanged("MatchText");
		}
	}

	public MainFrameViewModel()
	{
		BackCommand = new ReplayCommand(BackCommandHandler);
	}

	public override void LoadData()
	{
		HostProxy.deviceManager.DisConnecte += DeviceWatcher_DisConnecte;
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice != null && masterDevice.SoftStatus == DeviceSoftStateEx.Online)
		{
			ChangeCurrentDevice(masterDevice, _firstLoad: true);
		}
		else
		{
			Context.Switch(ViewType.START);
		}
		base.LoadData();
	}

	private void DeviceWatcher_DisConnecte(object sender, DeviceEx e)
	{
		if (CurrentDevice == null || (CurrentDevice != null && CurrentDevice.Identifer.Equals(e.Identifer)))
		{
			Context.CurrentRestoreViewType = ViewType.RESTOREMAIN;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				MatchText = "K0738";
				MatchVisibility = Visibility.Hidden;
				Context.Switch(ViewType.START);
			});
			Context.DisposeWorker();
			CurrentDevice = HostProxy.deviceManager.MasterDevice;
		}
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		DeviceEx device = (DeviceEx)sender;
		ChangeCurrentDevice(device);
	}

	private void ChangeCurrentDevice(DeviceEx device, bool _firstLoad = false)
	{
		bool num = device != null && device.ConnectType != ConnectType.Fastboot;
		if (Context.CurrentRestoreViewType == ViewType.BACKUPMAIN && Context.Level2Frame != null)
		{
			_ = Context.Level2Frame.CurrentView;
		}
		if (Context.CurrentRestoreViewType == ViewType.RESTOREMAIN && Context.Level2Frame != null)
		{
			_ = Context.Level2Frame.CurrentView;
		}
		if (!num)
		{
			CurrentDevice = null;
			Context.CurrentRestoreViewType = ViewType.RESTOREMAIN;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				MatchVisibility = Visibility.Hidden;
				Context.Switch(ViewType.START);
			});
			return;
		}
		CurrentDevice = device;
		if (!_firstLoad && device.ConnectedAppType == "Moto")
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				Context.Switch(ViewType.START);
			});
			if (device.SoftStatus == DeviceSoftStateEx.Online && HostProxy.HostNavigation.CurrentPluginID == "13f79fe4cfc98747c78794a943886bcd")
			{
				HostProxy.ViewContext.FindViewModel<StartViewModel>(typeof(StartView)).MotoHelperCheck();
			}
		}
		else if (device.SoftStatus == DeviceSoftStateEx.Online)
		{
			Context.CurrentRestoreViewType = ViewType.RESTOREMAIN;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				MatchVisibility = Visibility.Hidden;
				Context.Switch(ViewType.MAIN, null, reload: true);
			});
		}
		else if (device.SoftStatus == DeviceSoftStateEx.Offline)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				Context.Switch(ViewType.START);
				MatchText = "K0768";
			});
		}
	}

	private void BackCommandHandler(object data)
	{
		Context.Switch(ViewType.START);
	}
}
