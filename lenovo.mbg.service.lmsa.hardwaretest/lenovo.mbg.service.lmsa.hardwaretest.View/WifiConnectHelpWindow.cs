using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hardwaretest.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.hardwaretest.View;

public partial class WifiConnectHelpWindow : Window, IUserMsgControl, IComponentConnector, IStyleConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public WifiConnectHelpWindow()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		lv.Tag = HostProxy.LanguageService.IsChinaRegionAndLanguage();
		GlobalCmdHelper.Instance.CloseWifiTutorialEvent = delegate
		{
			base.Dispatcher.Invoke(delegate
			{
				Close();
				GlobalCmdHelper.Instance.CloseWifiTutorialEvent = null;
			});
		};
	}

	protected override void OnClosed(EventArgs e)
	{
		CloseAction?.Invoke(true);
		base.OnClosed(e);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnSelectedChanged(object sender, SelectionChangedEventArgs e)
	{
		WifiConnectHelpWindowModel wifiConnectHelpWindowModel = base.DataContext as WifiConnectHelpWindowModel;
		int selectedIndex = (sender as ListView).SelectedIndex;
		switch (selectedIndex)
		{
		case 0:
			wifiConnectHelpWindowModel.IsPrevBtnEnable = false;
			wifiConnectHelpWindowModel.IsNextBtnEnable = true;
			break;
		case 4:
			wifiConnectHelpWindowModel.IsPrevBtnEnable = true;
			wifiConnectHelpWindowModel.IsNextBtnEnable = false;
			break;
		default:
			wifiConnectHelpWindowModel.IsPrevBtnEnable = true;
			wifiConnectHelpWindowModel.IsNextBtnEnable = true;
			break;
		}
		switch (selectedIndex)
		{
		case 1:
			qrCode.Visibility = Visibility.Visible;
			wifiCode.Visibility = Visibility.Collapsed;
			encryptCode.Visibility = Visibility.Collapsed;
			break;
		case 4:
			qrCode.Visibility = Visibility.Collapsed;
			wifiCode.Visibility = Visibility.Visible;
			encryptCode.Visibility = Visibility.Visible;
			break;
		default:
			qrCode.Visibility = Visibility.Collapsed;
			wifiCode.Visibility = Visibility.Collapsed;
			encryptCode.Visibility = Visibility.Collapsed;
			break;
		}
	}

	private void ListViewItemSelected(object sender, RoutedEventArgs e)
	{
		if (sender is ListViewItem listViewItem)
		{
			listViewItem.BringIntoView();
		}
	}
}
