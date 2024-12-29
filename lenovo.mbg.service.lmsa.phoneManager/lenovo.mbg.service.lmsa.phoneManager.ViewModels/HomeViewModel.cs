using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class HomeViewModel : ViewModelBase
{
	private static HomeViewModel _singleInatance = null;

	private static object _locker = new object();

	private ReplayCommand goToRescueCommand;

	private string _modelName = string.Empty;

	private double _battery;

	private string _processor = string.Empty;

	private string _imei1 = "K0470";

	private string _imei1Title = string.Empty;

	private string _imei2 = string.Empty;

	private Visibility _imei2Visibility = Visibility.Collapsed;

	private string _sn = string.Empty;

	private string _updateTime = string.Empty;

	private string _androidVersion = string.Empty;

	private string _currentVersion = string.Empty;

	private double _internalTotal;

	private string internalTotalWithUnit;

	private double _internalUsed;

	private string internalUsedWithUnit;

	private double _internalFree;

	private string _internalSotrageFormatStr = "0KB " + HostProxy.LanguageService.Translate("K0469") + " 0KB";

	private double _externalTotal;

	private double _externalUsed;

	private double _externalFree;

	private static string _defaultExternalStorageStr = "K0470";

	private string _externalSotrageFormatStr = _defaultExternalStorageStr;

	private NavgationListViewModel _centerNavgationFirstLineItems;

	private ImageSource _screenImgSource;

	private Visibility _screencapVisibility = Visibility.Hidden;

	private bool _shotButtonEnabled;

	private bool _refreshButtonEnabled;

	private double _phoneFrameWidth;

	private double _phoneFrameHeight;

	private ReplayCommand warrentyClickCommand;

	private bool commboxStatusChangedByManual;

	private ReplayCommand otherDevicesButtonCommand;

	private DateTime _CurrentDateTime = DateTime.Now;

	private Visibility btn_IsEnabled;

	public static HomeViewModel SingleInstance
	{
		get
		{
			if (_singleInatance == null)
			{
				lock (_locker)
				{
					if (_singleInatance == null)
					{
						_singleInatance = new HomeViewModel();
					}
				}
			}
			return _singleInatance;
		}
	}

	public ReplayCommand GoToRescueCommand
	{
		get
		{
			return goToRescueCommand;
		}
		set
		{
			if (goToRescueCommand != value)
			{
				goToRescueCommand = value;
				OnPropertyChanged("GoToRescueCommand");
			}
		}
	}

	public string ModelName
	{
		get
		{
			return _modelName;
		}
		set
		{
			if (!(_modelName == value))
			{
				_modelName = value;
				OnPropertyChanged("ModelName");
			}
		}
	}

	public double Battery
	{
		get
		{
			return _battery;
		}
		set
		{
			if (_battery != value)
			{
				_battery = value;
				OnPropertyChanged("Battery");
			}
		}
	}

	public string Processor
	{
		get
		{
			return _processor;
		}
		set
		{
			if (!(_processor == value))
			{
				_processor = value;
				OnPropertyChanged("Processor");
			}
		}
	}

	public string IMEI1
	{
		get
		{
			return _imei1;
		}
		set
		{
			if (!(_imei1 == value))
			{
				if (string.IsNullOrEmpty(value) || value.Equals("null"))
				{
					_imei1 = "K0470";
				}
				else
				{
					_imei1 = value;
				}
				OnPropertyChanged("IMEI1");
			}
		}
	}

	public string Imei1Title
	{
		get
		{
			return _imei1Title;
		}
		set
		{
			if (!(_imei1Title == value))
			{
				_imei1Title = value;
				OnPropertyChanged("Imei1Title");
			}
		}
	}

	public string IMEI2
	{
		get
		{
			return _imei2;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value.Equals(IMEI1) || value.Equals("null"))
			{
				_imei2 = string.Empty;
				Imei1Title = "K0459";
				IMEI2Visibility = Visibility.Collapsed;
			}
			else
			{
				_imei2 = value;
				Imei1Title = "K0460";
				IMEI2Visibility = Visibility.Visible;
			}
			OnPropertyChanged("IMEI2");
		}
	}

	public Visibility IMEI2Visibility
	{
		get
		{
			return _imei2Visibility;
		}
		set
		{
			if (_imei2Visibility != value)
			{
				_imei2Visibility = value;
				OnPropertyChanged("IMEI2Visibility");
			}
		}
	}

	public string SN
	{
		get
		{
			return _sn;
		}
		set
		{
			if (!(_sn == value))
			{
				_sn = value;
				OnPropertyChanged("SN");
			}
		}
	}

	public string UpdateTime
	{
		get
		{
			return _updateTime;
		}
		set
		{
			if (!(_updateTime == value))
			{
				_updateTime = value;
				OnPropertyChanged("UpdateTime");
			}
		}
	}

	public string AndroidVersion
	{
		get
		{
			return _androidVersion;
		}
		set
		{
			if (!(_androidVersion == value))
			{
				_androidVersion = value;
				OnPropertyChanged("AndroidVersion");
			}
		}
	}

	public string CurrentVersion
	{
		get
		{
			return _currentVersion;
		}
		set
		{
			if (!(_currentVersion == value))
			{
				_currentVersion = value;
				OnPropertyChanged("CurrentVersion");
			}
		}
	}

	public double InternalTotal
	{
		get
		{
			return _internalTotal;
		}
		set
		{
			if (_internalTotal != value)
			{
				_internalTotal = value;
				OnPropertyChanged("InternalTotal");
			}
		}
	}

	public string InternalTotalWithUnit
	{
		get
		{
			return internalTotalWithUnit;
		}
		set
		{
			if (!(internalTotalWithUnit == value))
			{
				internalTotalWithUnit = value;
				OnPropertyChanged("InternalTotalWithUnit");
			}
		}
	}

	public double InternalUsed
	{
		get
		{
			return _internalUsed;
		}
		set
		{
			if (_internalUsed != value)
			{
				_internalUsed = value;
				OnPropertyChanged("InternalUsed");
			}
		}
	}

	public string InternalUsedWithUnit
	{
		get
		{
			return internalUsedWithUnit;
		}
		set
		{
			if (!(internalUsedWithUnit == value))
			{
				internalUsedWithUnit = value;
				OnPropertyChanged("InternalUsedWithUnit");
			}
		}
	}

	public double InternalFree
	{
		get
		{
			return _internalFree;
		}
		set
		{
			if (_internalFree != value)
			{
				_internalFree = value;
				OnPropertyChanged("InternalFree");
			}
		}
	}

	public string InternalStorageFormatStr
	{
		get
		{
			return _internalSotrageFormatStr;
		}
		set
		{
			if (!(_internalSotrageFormatStr == value))
			{
				_internalSotrageFormatStr = value;
				OnPropertyChanged("InternalStorageFormatStr");
			}
		}
	}

	public double ExternalTotal
	{
		get
		{
			return _externalTotal;
		}
		set
		{
			if (_externalTotal != value)
			{
				_externalTotal = value;
				OnPropertyChanged("ExternalTotal");
			}
		}
	}

	public double ExternalUsed
	{
		get
		{
			return _externalUsed;
		}
		set
		{
			if (_externalUsed != value)
			{
				_externalUsed = value;
				OnPropertyChanged("ExternalUsed");
			}
		}
	}

	public double ExternalFree
	{
		get
		{
			return _externalFree;
		}
		set
		{
			if (_externalFree != value)
			{
				_externalFree = value;
				OnPropertyChanged("ExternalFree");
			}
		}
	}

	public string ExternalStorageFormatStr
	{
		get
		{
			return _externalSotrageFormatStr;
		}
		set
		{
			if (!(_externalSotrageFormatStr == value))
			{
				_externalSotrageFormatStr = value;
				OnPropertyChanged("ExternalStorageFormatStr");
			}
		}
	}

	public NavgationListViewModel CenterNavgationFirstLineItems
	{
		get
		{
			return _centerNavgationFirstLineItems;
		}
		set
		{
			if (_centerNavgationFirstLineItems != value)
			{
				_centerNavgationFirstLineItems = value;
				OnPropertyChanged("CenterNavgationFirstLineItems");
			}
		}
	}

	public ReplayCommand FileManagementClickCommand { get; set; }

	public ReplayCommand BackupRestoreClickCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public ImageSource ScreenImgSource
	{
		get
		{
			return _screenImgSource;
		}
		set
		{
			if (_screenImgSource != value)
			{
				_screenImgSource = value;
				OnPropertyChanged("ScreenImgSource");
			}
		}
	}

	public Visibility ScreencapVisibility
	{
		get
		{
			return _screencapVisibility;
		}
		set
		{
			if (_screencapVisibility != value)
			{
				_screencapVisibility = value;
				OnPropertyChanged("ScreencapVisibility");
			}
		}
	}

	public bool ShotButtonEnabled
	{
		get
		{
			return _shotButtonEnabled;
		}
		set
		{
			if (_shotButtonEnabled != value)
			{
				_shotButtonEnabled = value;
				OnPropertyChanged("ShotButtonEnabled");
			}
		}
	}

	public bool RefreshButtonEnabled
	{
		get
		{
			return _refreshButtonEnabled;
		}
		set
		{
			if (_refreshButtonEnabled != value)
			{
				_refreshButtonEnabled = value;
				OnPropertyChanged("RefreshButtonEnabled");
			}
		}
	}

	public double PhoneFrameWidth
	{
		get
		{
			return _phoneFrameWidth;
		}
		set
		{
			if (_phoneFrameWidth != value)
			{
				_phoneFrameWidth = value;
				OnPropertyChanged("PhoneFrameWidth");
			}
		}
	}

	public double PhoneFrameHeight
	{
		get
		{
			return _phoneFrameHeight;
		}
		set
		{
			if (_phoneFrameHeight != value)
			{
				_phoneFrameHeight = value;
				OnPropertyChanged("PhoneFrameHeight");
			}
		}
	}

	public ReplayCommand RefreshCommand { get; set; }

	public ReplayCommand ShotCommand { get; set; }

	public ConnectedDeviceViewModel ConnectedDeviceViewModel => ConnectedDeviceViewModel.Instance;

	public ReplayCommand DeviceInfoCommand { get; set; }

	public ReplayCommand WarrentyClickCommand
	{
		get
		{
			return warrentyClickCommand;
		}
		set
		{
			if (warrentyClickCommand != value)
			{
				warrentyClickCommand = value;
				OnPropertyChanged("WarrentyClickCommand");
			}
		}
	}

	public bool CommboxStatusChangedByManual
	{
		get
		{
			return commboxStatusChangedByManual;
		}
		set
		{
			if (commboxStatusChangedByManual != value)
			{
				commboxStatusChangedByManual = value;
				OnPropertyChanged("CommboxStatusChangedByManual");
			}
		}
	}

	public ReplayCommand OtherDevicesButtonCommand
	{
		get
		{
			return otherDevicesButtonCommand;
		}
		set
		{
			if (otherDevicesButtonCommand != value)
			{
				otherDevicesButtonCommand = value;
				OnPropertyChanged("OtherDevicesButtonCommand");
			}
		}
	}

	public DateTime CurrentDateTime
	{
		get
		{
			return _CurrentDateTime;
		}
		set
		{
			_CurrentDateTime = value;
			OnPropertyChanged("CurrentDateTime");
		}
	}

	public Visibility IsEnabled
	{
		get
		{
			return btn_IsEnabled;
		}
		set
		{
			btn_IsEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	private HomeViewModel()
	{
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_OnMasterDeviceChanged;
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		ShotCommand = new ReplayCommand(ShotCommandHandler);
		DeviceInfoCommand = new ReplayCommand(DeviceInfoCommandHandler);
		WarrentyClickCommand = new ReplayCommand(WarrentyClickCommandHandler);
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		OtherDevicesButtonCommand = new ReplayCommand(OtherDevicesButtonCommandHandler);
		FileManagementClickCommand = new ReplayCommand(FileManagementClickCommandHandler);
		BackupRestoreClickCommand = new ReplayCommand(BackupRestoreClickCommandHandler);
		GoToRescueCommand = new ReplayCommand(GoToRescueCommandHandler);
	}

	private void DeviceManager_OnMasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_StatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_StatusChanged;
		}
	}

	private void Current_StatusChanged(object sender, DeviceSoftStateEx e)
	{
		switch (e)
		{
		case DeviceSoftStateEx.Online:
			ConnectedHandler();
			break;
		case DeviceSoftStateEx.Offline:
			ClearDeviceProperty();
			break;
		}
	}

	public override void LoadData()
	{
		base.LoadData();
	}

	private void GoToRescueCommandHandler(object args)
	{
		HostProxy.HostNavigation.SwitchTo("8ab04aa975e34f1ca4f9dc3a81374e2c", "mydevicerescue");
	}

	private void ConnectedHandler()
	{
		TcpAndroidDevice current = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (current != null)
		{
			commboxStatusChangedByManual = false;
			setDeviceProperty(current);
			if (ScreencapSwitch() && current.ConnectType == ConnectType.Adb)
			{
				RefreshScreen(string.Empty);
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				IsEnabled = ((current.ConnectType == ConnectType.Wifi) ? Visibility.Hidden : Visibility.Visible);
			});
		}
	}

	private void setDeviceProperty(TcpAndroidDevice device)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			IAndroidDevice property = device.Property;
			if (property == null)
			{
				ClearDeviceProperty();
			}
			else
			{
				ModelName = property.ModelName;
				Battery = property.BatteryQuantityPercentage;
				IMEI1 = property.IMEI1;
				IMEI2 = property.IMEI2;
				SN = property.SN;
				Processor = property.Processor;
				UpdateTime = property.Uptime;
				AndroidVersion = property.AndroidVersion;
				CurrentVersion = property.GetPropertyValue("ro.build.display.id");
				InternalTotal = property.TotalInternalStorage;
				InternalUsed = property.UsedInternalStorage;
				InternalUsedWithUnit = property.UsedInternalStorageWithUnit;
				InternalFree = property.FreeInternalStorage;
				InternalTotalWithUnit = property.TotalInternalStorageWithUnit;
				ExternalTotal = property.TotalExternalStorage;
				ExternalUsed = property.UsedExternalStorage;
				ExternalFree = property.FreeExternalStorage;
			}
		});
	}

	private void ClearDeviceProperty()
	{
	}

	private void LoadCenterNavgation()
	{
		CenterNavgationFirstLineItems = new NavgationListViewModel();
		CenterNavgationFirstLineItems.AddRange(new List<CenterNavButtonViewModel>
		{
			new CenterNavButtonViewModel
			{
				ButtonImageSource = (ImageSource)System.Windows.Application.Current.FindResource("AppBlueDrawingImage"),
				ButtonText = "K0474",
				ButtonTextDisplay = "K0474",
				ItemContainerMargin = new Thickness(0.0, 0.0, 52.0, 0.0),
				Count = 10
			},
			new CenterNavButtonViewModel
			{
				ButtonImageSource = (ImageSource)System.Windows.Application.Current.FindResource("PicBlueDrawingImage"),
				ButtonText = "K0475",
				ButtonTextDisplay = "K0475",
				ItemContainerMargin = new Thickness(0.0, 0.0, 52.0, 0.0),
				Count = 20
			},
			new CenterNavButtonViewModel
			{
				ButtonImageSource = (ImageSource)System.Windows.Application.Current.FindResource("MusicBlueDrawingImage"),
				ButtonTextDisplay = "K0476",
				ButtonText = "K0476"
			},
			new CenterNavButtonViewModel
			{
				ButtonImageSource = (ImageSource)System.Windows.Application.Current.FindResource("VideoBlueDrawingImage"),
				ButtonTextDisplay = "K0477",
				ButtonText = "K0477",
				ItemContainerMargin = new Thickness(0.0, 0.0, 52.0, 0.0)
			},
			new CenterNavButtonViewModel
			{
				ButtonImageSource = (ImageSource)System.Windows.Application.Current.FindResource("ContactBlueDrawingImage"),
				ButtonTextDisplay = "K0478",
				ButtonText = "K0478",
				ItemContainerMargin = new Thickness(0.0, 0.0, 52.0, 0.0)
			}
		});
	}

	private void FileManagementClickCommandHandler(object parameter)
	{
	}

	private void BackupRestoreClickCommandHandler(object parameter)
	{
	}

	public bool ScreencapSwitch()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		if (tcpAndroidDevice.PhysicalStatus == DevicePhysicalStateEx.Online && tcpAndroidDevice.ConnectType == ConnectType.Adb)
		{
			ScreencapVisibility = Visibility.Visible;
			ShotButtonEnabled = true;
			RefreshButtonEnabled = true;
			return true;
		}
		ShotButtonEnabled = false;
		RefreshButtonEnabled = false;
		ScreenImgSource = null;
		ScreencapVisibility = Visibility.Hidden;
		if (LeftNavigationViewModel.SingleInstance.SelectedItem != null && (LeftNavigationViewModel.SingleInstance.SelectedItem.Key.ToString() == "lmsa-plugin-Device-backuprestore" || LeftNavigationViewModel.SingleInstance.SelectedItem.Key.ToString() == "lmsa-plugin-Device-oneclickclone"))
		{
			LeftNavigationViewModel.SingleInstance.SelectedItem = LeftNavigationViewModel.SingleInstance.Items.FirstOrDefault((LeftNavigationItemViewModel n) => "lmsa-plugin-Device-home".Equals(n.Key));
		}
		return false;
	}

	public string Screencap()
	{
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		string screencapDir = Configurations.ScreencapDir;
		if (!Directory.Exists(screencapDir))
		{
			try
			{
				Directory.CreateDirectory(screencapDir);
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error($"Create new dir{screencapDir} throw exception:{ex.ToString()}");
				return string.Empty;
			}
		}
		if (tcpAndroidDevice.Property == null)
		{
			return string.Empty;
		}
		string internalStoragePath = tcpAndroidDevice.Property.InternalStoragePath;
		internalStoragePath = ((!string.IsNullOrEmpty(internalStoragePath) && internalStoragePath.Length != 0 && internalStoragePath[0] == '/') ? internalStoragePath.TrimEnd('/') : "/storage/emulated/0");
		string arg = $"{internalStoragePath}/screencap_lmsa.lmsapng";
		string text = Path.Combine(screencapDir, "screencap_lmsa_" + Guid.NewGuid().ToString("N") + ".png");
		if (File.Exists(text))
		{
			try
			{
				File.Delete(text);
			}
			catch
			{
			}
		}
		string arg2 = tcpAndroidDevice.DeviceOperator.Shell(tcpAndroidDevice.Identifer, $"screencap -p {arg}");
		LogHelper.LogInstance.Info($"Screen cap response {arg2}");
		string arg3 = tcpAndroidDevice.DeviceOperator.Command($"pull {arg} {text}", -1, tcpAndroidDevice.Identifer);
		LogHelper.LogInstance.Info($"Screen cap pull file response {arg3}");
		return text;
	}

	public void RefreshScreen(string imgPath)
	{
		RefreshButtonEnabled = false;
		Task.Factory.StartNew(delegate
		{
			try
			{
				if (string.IsNullOrEmpty(imgPath))
				{
					imgPath = Screencap();
				}
				if (File.Exists(imgPath))
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						try
						{
							using Image image = GlobalFun.CreateThumbnail(250, 460, imgPath);
							BitmapImage bitmapImage = new BitmapImage();
							using (MemoryStream memoryStream = new MemoryStream())
							{
								image.Save(memoryStream, ImageFormat.Png);
								memoryStream.Position = 0L;
								bitmapImage.BeginInit();
								bitmapImage.StreamSource = memoryStream;
								bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
								bitmapImage.EndInit();
								bitmapImage.Freeze();
							}
							ScreenImgSource = bitmapImage;
						}
						catch (Exception ex)
						{
							LogHelper.LogInstance.Error($"Create new bit map iamge {imgPath} throw exception:{ex.ToString()}");
						}
					});
				}
			}
			finally
			{
				RefreshButtonEnabled = true;
			}
		});
	}

	private void RefreshCommandHandler(object parameter)
	{
		RefreshScreen(string.Empty);
	}

	private void ShotCommandHandler(object parameter)
	{
		ShotButtonEnabled = false;
		Task.Factory.StartNew(delegate
		{
			try
			{
				string imgPath = Screencap();
				RefreshScreen(imgPath);
				if (File.Exists(imgPath))
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						FileInfo fileInfo = new FileInfo(imgPath);
						SaveFileDialog saveFileDialog = new SaveFileDialog
						{
							FileName = "screencap.png",
							InitialDirectory = fileInfo.DirectoryName,
							Filter = "PNG|*.png"
						};
						if (saveFileDialog.ShowDialog() == DialogResult.OK)
						{
							try
							{
								File.Copy(imgPath, saveFileDialog.FileName, overwrite: true);
							}
							catch (Exception ex)
							{
								LogHelper.LogInstance.Error($"Copy file[{imgPath}] throw exception:{ex.ToString()}");
							}
						}
					});
				}
			}
			finally
			{
				ShotButtonEnabled = true;
			}
		});
	}

	private void DeviceInfoCommandHandler(object data)
	{
		Window win = new DeviceInfoWindow
		{
			DataContext = this
		};
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
	}

	private void WarrentyClickCommandHandler(object args)
	{
		HostProxy.HostNavigation.SwitchTo("a6099126929a4e74ac86f1c2405dcb32");
	}

	private void CloseCommandHandler(object data)
	{
		(data as Window)?.Close();
	}

	private void OtherDevicesButtonCommandHandler(object args)
	{
		List<ConnectedDeviceModel> connectedDevices = ConnectedDeviceViewModel.Instance.GetConnectedDevices();
		if (connectedDevices != null && connectedDevices.Count > 0)
		{
			CommboxStatusChangedByManual = true;
		}
	}
}
