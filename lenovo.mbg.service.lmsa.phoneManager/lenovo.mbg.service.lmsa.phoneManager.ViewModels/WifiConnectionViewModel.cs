using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class WifiConnectionViewModel : ViewModelBase
{
	private ReplayCommand goToUSBConnectCommand;

	private ReplayCommand appDownlaodCommand;

	private ImageSource _qrImageSource;

	public ReplayCommand GoToUSBConnectCommand
	{
		get
		{
			return goToUSBConnectCommand;
		}
		set
		{
			if (goToUSBConnectCommand != value)
			{
				goToUSBConnectCommand = value;
				OnPropertyChanged("GoToUSBConnectCommand");
			}
		}
	}

	public ReplayCommand AppDownloadCommand
	{
		get
		{
			return appDownlaodCommand;
		}
		set
		{
			if (appDownlaodCommand != value)
			{
				appDownlaodCommand = value;
				OnPropertyChanged("AppDownloadCommand");
			}
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
			if (_qrImageSource != value)
			{
				_qrImageSource = value;
				OnPropertyChanged("QrImageSource");
			}
		}
	}

	private void AppDownloadCommandHandler(object args)
	{
		GlobalFun.OpenUrlByBrowser(HostProxy.LanguageService.IsChinaRegionAndLanguage() ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
	}

	private void GoToUSBConnectCommandHandler(object args)
	{
		HomePageFrameViewModel.Single.Switch(typeof(USBConnectHelper));
	}

	public WifiConnectionViewModel()
	{
		base.LoadData();
		GoToUSBConnectCommand = new ReplayCommand(GoToUSBConnectCommandHandler);
		AppDownloadCommand = new ReplayCommand(AppDownloadCommandHandler);
		OnWifiMonitoringEndPointChanged(HostProxy.deviceManager.WirelessWaitForConnectEndPoints);
		HostProxy.deviceManager.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
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
