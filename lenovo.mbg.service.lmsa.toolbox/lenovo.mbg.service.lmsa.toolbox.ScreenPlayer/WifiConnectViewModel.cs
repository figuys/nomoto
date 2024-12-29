using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class WifiConnectViewModel : NotifyBase
{
	private bool _IsBtnEnable;

	private string _IPAddress;

	private ImageSource _QrCodeImage;

	public ReplayCommand TutorialCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public bool? IsConnectedtoMA { get; set; }

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

	public string IPAddress
	{
		get
		{
			return _IPAddress;
		}
		set
		{
			_IPAddress = value;
			OnPropertyChanged("IPAddress");
		}
	}

	public ImageSource QrCodeImage
	{
		get
		{
			return _QrCodeImage;
		}
		set
		{
			_QrCodeImage = value;
			OnPropertyChanged("QrCodeImage");
		}
	}

	public WifiConnectViewModel(Window wnd)
	{
		WifiConnectViewModel wifiConnectViewModel = this;
		CloseCommand = new ReplayCommand(delegate(object param)
		{
			wifiConnectViewModel.IsConnectedtoMA = (bool?)param;
			if (HostProxy.deviceManager.MasterDevice != null)
			{
				HostProxy.deviceManager.MasterDevice.SoftStatusChanged -= wifiConnectViewModel.OnMasterDeviceOnLine;
			}
			HostProxy.deviceManager.MasterDeviceChanged -= wifiConnectViewModel.OnMasterDeviceChanged;
			HostProxy.deviceManager.WifiMonitoringEndPointChanged -= wifiConnectViewModel.OnWifiMonitoringEndPointChanged;
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				wnd.Close();
			});
		});
		TutorialCommand = new ReplayCommand(delegate
		{
			GifDisplayWindow view = new GifDisplayWindow();
			view.VM.Init("K1062", new Uri[3]
			{
				new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial1.gif"),
				new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial2.gif"),
				new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial3.gif")
			}, new string[3] { "K1063", "K1064", "K1065" });
			HostProxy.HostMaskLayerWrapper.New(view, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => view.ShowDialog());
		});
		IsBtnEnable = false;
		HostProxy.deviceManager.MasterDeviceChanged += OnMasterDeviceChanged;
		HostProxy.deviceManager.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
		OnWifiMonitoringEndPointChanged(HostProxy.deviceManager.WirelessWaitForConnectEndPoints);
	}

	private void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> ipGateWayArr)
	{
		if (ipGateWayArr == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append($"V:{Configurations.AppVersionCode}").Append(Environment.NewLine);
		stringBuilder.Append($"DV:{Configurations.AppMinVersionCodeOfMoto}").Append(Environment.NewLine);
		string text = string.Empty;
		foreach (Tuple<string, string> item in ipGateWayArr)
		{
			stringBuilder.Append("IP:").Append(item.Item1).Append(Environment.NewLine);
			stringBuilder.Append("GATEWAY:").Append(item.Item2).Append(Environment.NewLine);
			string[] array = item.Item1.Split(':');
			if (array.Length == 2)
			{
				text = text + array[0] + "; ";
			}
		}
		IPAddress = (string.IsNullOrEmpty(text) ? "No ip available" : text.Trim(' ').Trim(';'));
		try
		{
			MemoryStream stream = QrCodeUtility.GenerateQrCodeImageStream(stringBuilder.ToString());
			HostProxy.CurrentDispatcher.BeginInvoke((Action)delegate
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
				bitmapImage.EndInit();
				QrCodeImage = bitmapImage;
			});
		}
		catch (Exception)
		{
		}
	}

	private void OnMasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += OnMasterDeviceOnLine;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= OnMasterDeviceOnLine;
		}
	}

	private void OnMasterDeviceOnLine(object sender, DeviceSoftStateEx e)
	{
		if (e == DeviceSoftStateEx.Online)
		{
			CloseCommand.Execute(false);
		}
	}
}
