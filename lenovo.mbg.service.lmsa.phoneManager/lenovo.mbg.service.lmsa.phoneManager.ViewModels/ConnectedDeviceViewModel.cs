using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ConnectedDeviceViewModel : ViewModelBase
{
	private static object lockerDevice = new object();

	private static object locker = new object();

	private static ConnectedDeviceViewModel _instance = null;

	private ProgressLoadingWindow _win;

	private ConnectedDeviceModel selectedItem;

	public static ConnectedDeviceViewModel Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (locker)
				{
					if (_instance == null)
					{
						_instance = new ConnectedDeviceViewModel();
					}
				}
			}
			return _instance;
		}
	}

	private ObservableCollection<ConnectedDeviceModel> m_ConnectedDevices { get; set; }

	public ObservableCollection<ConnectedDeviceModel> ConnectedDevices
	{
		get
		{
			return m_ConnectedDevices;
		}
		set
		{
			m_ConnectedDevices = value;
			OnPropertyChanged("ConnectedDevices");
		}
	}

	public RelayCommand<ConnectedDeviceModel> SelectionChangedCommand { get; private set; }

	public ConnectedDeviceModel SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			if (selectedItem != value)
			{
				selectedItem = value;
				if (selectedItem != null)
				{
					HostProxy.deviceManager.SwitchDevice(selectedItem.Device.Identifer);
				}
				OnPropertyChanged("SelectedItem");
			}
		}
	}

	public ConnectedDeviceViewModel()
	{
		m_ConnectedDevices = new ObservableCollection<ConnectedDeviceModel>();
		HostProxy.deviceManager.Connecte += DeviceManager_Connecte;
		HostProxy.deviceManager.DisConnecte += DeviceManager_DisConnecte;
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null && e.Current.ConnectType != ConnectType.Wifi)
		{
			e.Current.SoftStatusChanged += E_SoftStatusChanged;
		}
		if (e.Previous != null && e.Previous.ConnectType != ConnectType.Wifi)
		{
			e.Previous.SoftStatusChanged -= E_SoftStatusChanged;
		}
		Thread.Sleep(200);
		SelectMasterDevice(e.Current);
	}

	private void SelectMasterDevice(DeviceEx device)
	{
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice == null || masterDevice != device)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			ConnectedDeviceModel connectedDeviceModel = ConnectedDevices.FirstOrDefault((ConnectedDeviceModel n) => n.Key == masterDevice.Identifer);
			string sN = ((masterDevice.Property == null) ? string.Empty : masterDevice.Property.SN);
			string value = ((masterDevice.Property == null) ? string.Empty : masterDevice.Property.ModelName);
			if (connectedDeviceModel == null)
			{
				ConnectedDevices.Add(new ConnectedDeviceModel
				{
					Device = masterDevice,
					Key = masterDevice.Identifer,
					SN = sN,
					Value = value
				});
			}
			else if (connectedDeviceModel.Device.SoftStatus == DeviceSoftStateEx.Online)
			{
				connectedDeviceModel.SN = sN;
				connectedDeviceModel.Value = value;
			}
			foreach (ConnectedDeviceModel connectedDevice in GetConnectedDevices())
			{
				if (connectedDevice.Key.Equals(device.Identifer))
				{
					SelectedItem = connectedDevice;
					break;
				}
			}
		});
	}

	private void DeviceManager_DisConnecte(object sender, DeviceEx e)
	{
		e.SoftStatusChanged -= E_SoftStatusChanged;
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			for (int num = ConnectedDevices.Count - 1; num >= 0; num--)
			{
				if (ConnectedDevices[num].Key.Equals(e.Identifer))
				{
					ConnectedDevices.RemoveAt(num);
				}
			}
		});
	}

	private void DeviceManager_Connecte(object sender, DeviceEx e)
	{
		if (e.ConnectType == ConnectType.Adb || e.ConnectType == ConnectType.Wifi)
		{
			e.SoftStatusChanged += E_SoftStatusChanged;
		}
	}

	private void E_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		TcpAndroidDevice d = sender as TcpAndroidDevice;
		switch (e)
		{
		case DeviceSoftStateEx.Online:
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				if (ConnectedDevices.FirstOrDefault((ConnectedDeviceModel n) => n.Key == d.Identifer) == null)
				{
					ConnectedDeviceModel item2 = new ConnectedDeviceModel
					{
						Device = d,
						Key = d.Identifer,
						SN = d.Property.SN,
						Value = d.Property.ModelName
					};
					ConnectedDevices.Add(item2);
				}
				SelectMasterDevice(d);
				_ = (ImageSource)Application.Current.Resources.FindName("bottom_usbDrawingImage");
			});
			break;
		case DeviceSoftStateEx.Offline:
			if (ConnectedDevices.Count <= 0)
			{
				break;
			}
			{
				foreach (ConnectedDeviceModel item in ConnectedDevices.ToList())
				{
					if (item.Key.Equals(d.Identifer))
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							ConnectedDevices.Remove(item);
						});
					}
				}
				break;
			}
		}
	}

	private void Close()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (_win != null)
			{
				_win.Close();
				_win = null;
			}
		});
	}

	public List<ConnectedDeviceModel> GetConnectedDevices()
	{
		lock (lockerDevice)
		{
			if (ConnectedDevices != null)
			{
				return ConnectedDevices.ToList();
			}
			return null;
		}
	}
}
