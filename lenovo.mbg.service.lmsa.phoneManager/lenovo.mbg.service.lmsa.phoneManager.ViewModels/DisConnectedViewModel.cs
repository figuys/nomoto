using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class DisConnectedViewModel : ViewModelBase
{
	private ImageSource _qrImageSource;

	private string _ipAddressInfo = string.Empty;

	private static DisConnectedViewModel _singleInstance;

	private static object locker = new object();

	private Visibility disconnectedPanelVisibility;

	private Visibility usbHelperPanelVisibility;

	private Visibility wifiConnectPanelVisibility;

	private ReplayCommand goToUSBHelperPanelCommand;

	private ReplayCommand goToWIFIConnectPanelCommand;

	private bool isPopWindowMode;

	private object popWindow;

	public ImageSource QrImageSource
	{
		get
		{
			return _qrImageSource;
		}
		set
		{
			if (_qrImageSource != value)
			{
				_qrImageSource = value;
				OnPropertyChanged("QrImageSource");
			}
		}
	}

	public string IPAddressInfo
	{
		get
		{
			return _ipAddressInfo;
		}
		set
		{
			if (!(_ipAddressInfo == value))
			{
				_ipAddressInfo = value;
				OnPropertyChanged("IPAddressInfo");
			}
		}
	}

	public static DisConnectedViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				lock (locker)
				{
					if (_singleInstance == null)
					{
						_singleInstance = new DisConnectedViewModel();
					}
				}
			}
			return _singleInstance;
		}
	}

	public Visibility DisconnectedPanelVisibility
	{
		get
		{
			return disconnectedPanelVisibility;
		}
		set
		{
			if (disconnectedPanelVisibility != value)
			{
				disconnectedPanelVisibility = value;
				OnPropertyChanged("DisconnectedPanelVisibility");
			}
		}
	}

	public Visibility USBHelperPanelVisibility
	{
		get
		{
			return usbHelperPanelVisibility;
		}
		set
		{
			if (usbHelperPanelVisibility != value)
			{
				usbHelperPanelVisibility = value;
				OnPropertyChanged("USBHelperPanelVisibility");
			}
		}
	}

	public Visibility WIFIConnectPanelVisibility
	{
		get
		{
			return wifiConnectPanelVisibility;
		}
		set
		{
			if (wifiConnectPanelVisibility != value)
			{
				wifiConnectPanelVisibility = value;
				OnPropertyChanged("WIFIConnectPanelVisibility");
			}
		}
	}

	public ReplayCommand GoToUSBHelperPanelCommand
	{
		get
		{
			return goToUSBHelperPanelCommand;
		}
		set
		{
			if (goToUSBHelperPanelCommand != value)
			{
				goToUSBHelperPanelCommand = value;
				OnPropertyChanged("GoToUSBHelperPanelCommand");
			}
		}
	}

	public ReplayCommand GoToWIFIConnectPanelCommand
	{
		get
		{
			return goToWIFIConnectPanelCommand;
		}
		set
		{
			if (goToWIFIConnectPanelCommand != value)
			{
				goToWIFIConnectPanelCommand = value;
				OnPropertyChanged("GoToWIFIConnectPanelCommand");
			}
		}
	}

	public bool IsPopWindowMode
	{
		get
		{
			return isPopWindowMode;
		}
		set
		{
			if (isPopWindowMode != value)
			{
				isPopWindowMode = value;
				OnPropertyChanged("IsPopWindowMode");
			}
		}
	}

	public object PopWindow
	{
		get
		{
			return popWindow;
		}
		set
		{
			if (popWindow != value)
			{
				popWindow = value;
				OnPropertyChanged("PopWindow");
			}
		}
	}

	public DisConnectedViewModel()
	{
		base.LoadData();
		OnWifiMonitoringEndPointChanged(HostProxy.deviceManager.WirelessWaitForConnectEndPoints);
		HostProxy.deviceManager.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
		goToUSBHelperPanelCommand = new ReplayCommand(goToUSBHelperPanelCommandHandler);
		GoToWIFIConnectPanelCommand = new ReplayCommand(GoToWIFIConnectPanelCommandHandler);
	}

	private void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> ipGateWayArr)
	{
		if (ipGateWayArr == null)
		{
			return;
		}
		List<JsonEndPoint> EPs = new List<JsonEndPoint>(ipGateWayArr.Count);
		foreach (Tuple<string, string> item in ipGateWayArr)
		{
			string[] array = item.Item1.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Count() >= 2)
			{
				EPs.Add(new JsonEndPoint(array[0], int.Parse(array[1])));
			}
		}
		try
		{
			HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
			{
				string text = JsonHelper.SerializeObject2Json(new JsonEndPoints(EPs));
				MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(text);
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
				bitmapImage.EndInit();
				try
				{
					QrImageSource = bitmapImage;
					IPAddressInfo = text;
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

	private void goToUSBHelperPanelCommandHandler(object args)
	{
		HomePageFrameViewModel.Single.Switch(typeof(USBConnectHelper));
	}

	private void GoToWIFIConnectPanelCommandHandler(object args)
	{
		LogHelper.LogInstance.Info("User click wifi connect button, wifi connection page will show!");
		HomePageFrameViewModel.Single.Switch(typeof(WifiConnection));
	}
}
