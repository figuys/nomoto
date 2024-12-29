using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.Tutorials;

public partial class TutorialsWindow : Window, IUserMsgControl, IComponentConnector, IStyleConnector
{
	private bool isWifiTutorial;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public TutorialsWindow()
	{
		InitializeComponent();
		isWifiTutorial = false;
		base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		base.Owner = Application.Current.MainWindow;
		lv.Tag = HostProxy.LanguageService.IsChinaRegionAndLanguage();
	}

	private void MouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	public void SetWifiTutorial()
	{
		isWifiTutorial = true;
		txtTitle.LangKey = "K1464";
		HostProxy.deviceManager.WifiMonitoringEndPointChanged += OnWifiMonitoringEndPointChanged;
		OnWifiMonitoringEndPointChanged(HostProxy.deviceManager.WirelessWaitForConnectEndPoints);
		GlobalCmdHelper.Instance.CloseWifiTutorialEvent = delegate
		{
			base.Dispatcher.Invoke(delegate
			{
				OnBtnClose(null, null);
				GlobalCmdHelper.Instance.CloseWifiTutorialEvent = null;
			});
		};
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
		try
		{
			MemoryStream stream = QrCodeUtility.GenerateQrCodeImageStream(stringBuilder.ToString());
			HostProxy.CurrentDispatcher.BeginInvoke((Action)delegate
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
				bitmapImage.EndInit();
				qrCode.Source = bitmapImage;
			});
		}
		catch
		{
		}
	}

	private void ListViewItemSelected(object sender, RoutedEventArgs e)
	{
		if (sender is ListViewItem listViewItem)
		{
			listViewItem.BringIntoView();
		}
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		HostProxy.deviceManager.WifiMonitoringEndPointChanged -= OnWifiMonitoringEndPointChanged;
		Result = true;
		Close();
		CloseAction?.Invoke(true);
	}

	private void OnSelectedChanged(object sender, SelectionChangedEventArgs e)
	{
		ListView listView = sender as ListView;
		if (isWifiTutorial)
		{
			if (listView.SelectedIndex == 3)
			{
				qrCode.Visibility = Visibility.Visible;
			}
			else
			{
				qrCode.Visibility = Visibility.Collapsed;
			}
		}
	}
}
