using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class HomeViewModel : ViewModelBase
{
	protected string WarrantyParams;

	protected static WarrantyService warrantyService = new WarrantyService();

	private long shotlocker;

	private long refreshlocker;

	private ObservableCollection<DeviceInfoModel> _DeviceInfos;

	private double _InternaUsedPer;

	private string _InternaTotal;

	private string _InternaUsed;

	private bool isCopied;

	private bool _IsPhone;

	private bool _IsTablet;

	private bool _IsWifi;

	private ImageSource _ScreenImgSource;

	public Visibility _Visibile = Visibility.Hidden;

	public WarrantyInfoBaseViewModelV6 WarrantyBaseVm { get; set; }

	public ObservableCollection<DeviceInfoModel> DeviceInfos
	{
		get
		{
			return _DeviceInfos;
		}
		set
		{
			_DeviceInfos = value;
			OnPropertyChanged("DeviceInfos");
		}
	}

	public double InternaUsedPer
	{
		get
		{
			return _InternaUsedPer;
		}
		set
		{
			_InternaUsedPer = value;
			OnPropertyChanged("InternaUsedPer");
		}
	}

	public string InternaTotal
	{
		get
		{
			return _InternaTotal;
		}
		set
		{
			_InternaTotal = value;
			OnPropertyChanged("InternaTotal");
		}
	}

	public string InternaUsed
	{
		get
		{
			return _InternaUsed;
		}
		set
		{
			_InternaUsed = value;
			OnPropertyChanged("InternaUsed");
		}
	}

	public ReplayCommand CopyCommand { get; }

	public ReplayCommand StartRescueCommand { get; }

	public ReplayCommand ClickCommand { get; }

	protected IAndroidDevice properties { get; set; }

	public bool IsCopied
	{
		get
		{
			return isCopied;
		}
		set
		{
			isCopied = value;
			OnPropertyChanged("IsCopied");
		}
	}

	public bool IsPhone
	{
		get
		{
			return _IsPhone;
		}
		set
		{
			_IsPhone = value;
			OnPropertyChanged("IsPhone");
		}
	}

	public bool IsTablet
	{
		get
		{
			return _IsTablet;
		}
		set
		{
			_IsTablet = value;
			IsPhone = !_IsTablet;
			OnPropertyChanged("IsTablet");
		}
	}

	public bool IsWifi
	{
		get
		{
			return _IsWifi;
		}
		set
		{
			_IsWifi = value;
			OnPropertyChanged("IsWifi");
		}
	}

	public ImageSource ScreenImgSource
	{
		get
		{
			return _ScreenImgSource;
		}
		set
		{
			_ScreenImgSource = value;
			OnPropertyChanged("ScreenImgSource");
		}
	}

	public Visibility Visibile
	{
		get
		{
			return _Visibile;
		}
		set
		{
			_Visibile = value;
			OnPropertyChanged("Visibile");
		}
	}

	public HomeViewModel()
	{
		DeviceInfos = new ObservableCollection<DeviceInfoModel>();
		WarrantyBaseVm = new WarrantyInfoBaseViewModelV6(detail: false);
		CopyCommand = new ReplayCommand(CopyCommandHandler);
		StartRescueCommand = new ReplayCommand(StartRescueCommandHandler);
		ClickCommand = new ReplayCommand(ClickCommandHandler);
	}

	public override void LoadData(object data)
	{
		LoadDeviceInfo();
		base.LoadData(data);
	}

	public void LoadDeviceInfo()
	{
		DeviceEx currentDevice = Context.CurrentDevice;
		if (currentDevice == null)
		{
			return;
		}
		properties = currentDevice.Property;
		if (properties != null)
		{
			IsTablet = properties.Category == "tablet";
			IsWifi = currentDevice.ConnectType == ConnectType.Wifi;
			WarrantyParams = (IsTablet ? properties.SN : properties.IMEI1);
			ClickCommandHandler(1);
			Task.Run(delegate
			{
				LoadWarranty(properties.IMEI1, properties.SN);
			});
			InternaTotal = properties.TotalInternalStorageWithUnit;
			InternaUsed = properties.UsedInternalStorageWithUnit;
			InternaUsedPer = properties.UsedInternalStorage * 100 / properties.TotalInternalStorage;
			List<DeviceInfoModel> list = new List<DeviceInfoModel>();
			list.Add(new DeviceInfoModel(null, "K0455", properties.ModelName, 3, 0));
			list.Add(new DeviceInfoModel(null, "K0457", properties.BatteryQuantityPercentage + "%", 2, (int)properties.BatteryQuantityPercentage / 4));
			list.Add(new DeviceInfoModel(null, "K0458", properties.Processor, 3, 0));
			list.Add(new DeviceInfoModel(null, "K0460", properties.IMEI1, 1, 0));
			list.Add(new DeviceInfoModel(null, "K0461", properties.IMEI2, 1, 0));
			list.Add(new DeviceInfoModel(null, "K0462", properties.SN, 1, 0));
			list.Add(new DeviceInfoModel(null, "K0463", properties.FreeInternalStorageWithUnit + " " + HostProxy.LanguageService.Translate("K0469") + " " + properties.TotalInternalStorageWithUnit, 3, 0));
			if (properties.TotalExternalStorage > 0)
			{
				list.Add(new DeviceInfoModel(null, "K1378", properties.FreeExternalStorageWithUnit + " " + HostProxy.LanguageService.Translate("K0469") + " " + properties.TotalExternalStorageWithUnit, 3, 0));
			}
			list.Add(new DeviceInfoModel(null, "K0468", properties.AndroidVersion, 3, 0));
			list.Add(new DeviceInfoModel(null, "K0467", properties.GetPropertyValue("ro.build.display.id"), 3, 0));
			List<DeviceInfoModel> list2 = list.Where((DeviceInfoModel n) => !string.IsNullOrEmpty(n.Item2)).ToList();
			DeviceInfos.Clear();
			list2.ForEach(delegate(DeviceInfoModel n)
			{
				DeviceInfos.Add(n);
			});
		}
	}

	public async void LoadWarranty(string imei, string sn)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(() => Visibile = Visibility.Hidden);
		WarrantyInfoBaseModel warrantyInfoBaseModel = null;
		string text = (IsTablet ? sn : imei);
		if (!string.IsNullOrEmpty(text))
		{
			warrantyInfoBaseModel = await warrantyService.GetWarrantyInfo<WarrantyInfoBaseModel>(text);
			WarrantyBaseVm.UpdateModel(warrantyInfoBaseModel);
			System.Windows.Application.Current.Dispatcher.Invoke(() => Visibile = Visibility.Visible);
		}
		if (IsPhone && warrantyInfoBaseModel != null)
		{
			warrantyInfoBaseModel.imei = imei;
			HostProxy.HostOperationService.ShowBannerAsync(warrantyInfoBaseModel);
		}
	}

	protected void StartRescueCommandHandler(object data)
	{
		HostProxy.HostNavigation.SwitchTo("8ab04aa975e34f1ca4f9dc3a81374e2c", "mydevicerescue");
	}

	protected async void ClickCommandHandler(object aparams)
	{
		switch (int.Parse(aparams.ToString()))
		{
		case 0:
			if (Interlocked.Read(ref shotlocker) == 0L)
			{
				await ShotScreen();
				Interlocked.Exchange(ref shotlocker, 0L);
			}
			break;
		case 1:
			if (Interlocked.Read(ref refreshlocker) == 0L)
			{
				await RefreshScreen(string.Empty);
				Interlocked.Exchange(ref refreshlocker, 0L);
			}
			break;
		}
	}

	protected void CopyCommandHandler(object args)
	{
		DeviceInfoModel data = args as DeviceInfoModel;
		System.Windows.Clipboard.SetDataObject(data.Item2);
		data.Item5 = true;
		Task.Run(delegate
		{
			Thread.Sleep(800);
		}).ContinueWith((Task s) => HostProxy.CurrentDispatcher.Invoke(() => data.Item5 = false));
	}

	public string Screencap()
	{
		TcpAndroidDevice tcpAndroidDevice = Context.CurrentDevice as TcpAndroidDevice;
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
		if (tcpAndroidDevice.Property == null || tcpAndroidDevice.ConnectType == ConnectType.Wifi)
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

	public Task ShotScreen()
	{
		return Task.Run(delegate
		{
			string imgPath = Screencap();
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
		});
	}

	public Task RefreshScreen(string imgPath)
	{
		return Task.Factory.StartNew(delegate
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
		});
	}
}
