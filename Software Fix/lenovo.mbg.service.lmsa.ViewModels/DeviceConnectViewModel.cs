using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ViewV6;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class DeviceConnectViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	protected string PluginId;

	private Visibility _PopIconVisible = Visibility.Collapsed;

	private bool _IsOpen = false;

	private DeviceViewModel _SelectedItem;

	private int _Status = -1;

	private bool _IsEnabled = true;

	protected static object locker = new object();

	private static DeviceConnectViewModel _Instance;

	public WaitTips m_loadingWindow { get; private set; }

	public ObservableCollection<DeviceViewModel> ConnectedDevices { get; set; }

	public ReplayCommand CurDeviceClick { get; }

	protected string MasterDeviceId => global::Smart.DeviceManagerEx.MasterDevice?.Identifer;

	public Visibility PopIconVisible
	{
		get
		{
			return _PopIconVisible;
		}
		set
		{
			_PopIconVisible = value;
			OnPropertyChanged("PopIconVisible");
		}
	}

	public bool IsOpen
	{
		get
		{
			return _IsOpen;
		}
		set
		{
			_IsOpen = value;
			OnPropertyChanged("IsOpen");
		}
	}

	public DeviceViewModel SelectedItem
	{
		get
		{
			return _SelectedItem;
		}
		set
		{
			if (_SelectedItem == value)
			{
				return;
			}
			if (InMyDevice && _SelectedItem != null && _SelectedItem.Device.SoftStatus == DeviceSoftStateEx.Online && value != null && value.ConnectTy == ConnectType.Fastboot)
			{
				value.IsSelected = false;
				_SelectedItem.IsSelected = true;
				ApplcationClass.ApplcationStartWindow.ShowMessage("K1451");
				OnPropertyChanged("SelectedItem");
				return;
			}
			if (_SelectedItem != null)
			{
				_SelectedItem.IsSelected = false;
			}
			_SelectedItem = value;
			IsOpen = false;
			if (_SelectedItem != null)
			{
				_SelectedItem.IsSelected = true;
				Status = ((!_SelectedItem.IsMotorola) ? 1 : 2);
				if (MasterDeviceId != _SelectedItem.Id && (_SelectedItem.Device.ConnectType == ConnectType.Adb || _SelectedItem.Device.ConnectType == ConnectType.Wifi))
				{
					global::Smart.DeviceManagerEx.SwitchDevice(_SelectedItem.Id);
				}
			}
			else
			{
				DeviceViewModel deviceViewModel = ConnectedDevices.FirstOrDefault((DeviceViewModel n) => n.Id == MasterDeviceId);
				if (deviceViewModel == null)
				{
					deviceViewModel = ConnectedDevices.FirstOrDefault();
				}
				if (deviceViewModel != null)
				{
					_SelectedItem = deviceViewModel;
					_SelectedItem.IsSelected = true;
					if (MasterDeviceId == deviceViewModel.Id)
					{
						Status = ((!deviceViewModel.IsMotorola) ? 1 : 2);
					}
				}
				else
				{
					Status = -1;
				}
			}
			OnPropertyChanged("SelectedItem");
		}
	}

	public bool InMyDevice => "02928af025384c75ae055aa2d4f256c8" == PluginId;

	public int Status
	{
		get
		{
			return _Status;
		}
		set
		{
			_Status = value;
			OnPropertyChanged("Status");
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _IsEnabled;
		}
		set
		{
			_IsEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public static DeviceConnectViewModel Instance
	{
		get
		{
			if (_Instance == null)
			{
				lock (locker)
				{
					if (_Instance == null)
					{
						_Instance = new DeviceConnectViewModel();
					}
				}
			}
			return _Instance;
		}
	}

	private DeviceConnectViewModel()
	{
		ConnectedDevices = new ObservableCollection<DeviceViewModel>();
		ConnectedDevices.CollectionChanged += ConnectedDevices_CollectionChanged;
		CurDeviceClick = new ReplayCommand(CurDeviceClickHandler);
		global::Smart.DeviceManagerEx.Connecte += FireConnect;
		global::Smart.DeviceManagerEx.DisConnecte += FireDisConnect;
	}

	private void ConnectedDevices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		PopIconVisible = ((ConnectedDevices.Count <= 1) ? Visibility.Collapsed : Visibility.Visible);
	}

	public void ChangeEnadbled(string plubinId)
	{
		PluginId = plubinId;
		if (plubinId == "02928af025384c75ae055aa2d4f256c8")
		{
			IsEnabled = true;
		}
		else
		{
			IsEnabled = false;
		}
	}

	public override void Reset()
	{
		Status = -1;
		ConnectedDevices.Clear();
		_Instance = null;
	}

	private void CurDeviceClickHandler(object data)
	{
		IsOpen = !IsOpen;
	}

	private void FireConnect(object sender, DeviceEx e)
	{
		e.SoftStatusChanged += FireSoftStatusChanged;
		e.InstallAppCallback = delegate
		{
			if (e.PhysicalStatus == DevicePhysicalStateEx.Online)
			{
				InstallMADialogV6 _installView = null;
				Application.Current.Dispatcher.Invoke(delegate
				{
					_installView = new InstallMADialogV6();
				});
				if (ApplcationClass.ApplcationStartWindow.ShowMessage(_installView) != true)
				{
					ApplcationClass.ApplcationStartWindow.ShowMessage("K1449", "K1450", "K1448", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
					return false;
				}
				Task.Run(delegate
				{
					Application.Current.Dispatcher.Invoke(delegate
					{
						m_loadingWindow = new WaitTips("K1840");
						HostProxy.HostMaskLayerWrapper.New(m_loadingWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => m_loadingWindow.ShowDialog());
					});
				});
				return true;
			}
			return false;
		};
		e.RetryConnectCallback = delegate
		{
		};
	}

	protected string GenerateKey(DeviceEx device)
	{
		return device.Identifer;
	}

	private void RemoveDevice(DeviceEx device)
	{
		DeviceViewModel found = ConnectedDevices.FirstOrDefault((DeviceViewModel n) => n.Id == GenerateKey(device));
		Application.Current.Dispatcher.Invoke(delegate
		{
			if (found != null)
			{
				ConnectedDevices.Remove(found);
			}
			if (ConnectedDevices.Count == 0)
			{
				Status = -1;
			}
		});
	}

	private void AddOrUpdateDevice(DeviceEx device, string modelName, bool ismotorola)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			string key = GenerateKey(device);
			DeviceViewModel deviceViewModel = new DeviceViewModel(key, modelName, device.ConnectType, device, ismotorola);
			DeviceViewModel deviceViewModel2 = ConnectedDevices.FirstOrDefault((DeviceViewModel n) => n.Id == key);
			if (deviceViewModel2 == null)
			{
				ConnectedDevices.Add(deviceViewModel);
			}
			if (SelectedItem == null || (InMyDevice && SelectedItem.Device.ConnectType == ConnectType.Fastboot))
			{
				SelectedItem = deviceViewModel;
			}
		});
	}

	private void FireDisConnect(object sender, DeviceEx e)
	{
		e.SoftStatusChanged -= FireSoftStatusChanged;
		e.InstallAppCallback = null;
		RemoveDevice(e);
	}

	private void FireSoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		switch (e)
		{
		case DeviceSoftStateEx.Connecting:
		case DeviceSoftStateEx.Reconncting:
			if (SelectedItem == null)
			{
				Status = 0;
			}
			break;
		case DeviceSoftStateEx.Online:
		{
			DeviceEx deviceEx = sender as DeviceEx;
			string text = deviceEx.Property.ModelName;
			bool ismotorola = true;
			if (deviceEx.ConnectType == ConnectType.Fastboot)
			{
				string fingerPrint = deviceEx.Property.FingerPrint;
				if (!string.IsNullOrEmpty(fingerPrint) && fingerPrint.StartsWith("lenovo", StringComparison.CurrentCultureIgnoreCase))
				{
					ismotorola = false;
				}
			}
			else
			{
				ismotorola = deviceEx.Property.Brand?.Equals("motorola", StringComparison.CurrentCultureIgnoreCase) ?? false;
			}
			if (text != null)
			{
				text = ((!text.StartsWith("lenovo", StringComparison.CurrentCultureIgnoreCase)) ? Regex.Replace(text, "^motorola", "", RegexOptions.IgnoreCase).Trim() : Regex.Replace(text, "^lenovo", "", RegexOptions.IgnoreCase).Trim());
			}
			AddOrUpdateDevice(deviceEx, text, ismotorola);
			break;
		}
		case DeviceSoftStateEx.Offline:
		case DeviceSoftStateEx.ManualDisconnect:
			Application.Current.Dispatcher.Invoke(delegate
			{
				m_loadingWindow?.Close();
			});
			RemoveDevice(sender as DeviceEx);
			break;
		default:
			if (ConnectedDevices.Count == 0)
			{
				Status = -1;
			}
			break;
		}
	}

	public void ShowConnectedDeviceIcon()
	{
		if (ConnectedDevices.Count > 0)
		{
			DeviceViewModel deviceViewModel = ConnectedDevices.FirstOrDefault((DeviceViewModel m) => m.Id == MasterDeviceId) ?? ConnectedDevices.FirstOrDefault();
			if (deviceViewModel != null)
			{
				Status = ((!deviceViewModel.IsMotorola) ? 1 : 2);
			}
		}
	}
}
