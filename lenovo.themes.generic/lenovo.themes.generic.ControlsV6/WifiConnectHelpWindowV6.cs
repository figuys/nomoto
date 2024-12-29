using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ControlsV6;

public partial class WifiConnectHelpWindowV6 : Window, IUserMsgControl, IComponentConnector, IStyleConnector
{
	private bool _IsHwPop;

	private bool _IsHideQrCode;

	private WifiTutorialsType m_WifiTutorialsType;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public WifiConnectHelpWindowV6(bool isHwPop = false, string _titleKey = "", WifiTutorialsType _wifiType = WifiTutorialsType.RESCUE_PHONE)
	{
		InitializeComponent();
		_IsHwPop = isHwPop;
		m_WifiTutorialsType = _wifiType;
		txtRandomCode.Text = RandomAesKeyHelper.Instance.EncryptCode;
		base.Owner = Application.Current.MainWindow;
		lv.Tag = global::Smart.LanguageService.IsChinaRegionAndLanguage();
		if (!string.IsNullOrEmpty(_titleKey))
		{
			_IsHideQrCode = true;
			txtTitle.LangKey = _titleKey;
		}
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
		WifiConnectHelpWindowModelV6 wifiConnectHelpWindowModelV = base.DataContext as WifiConnectHelpWindowModelV6;
		ListView obj = sender as ListView;
		int selectedIndex = obj.SelectedIndex;
		_ = obj.SelectedItem;
		if (wifiConnectHelpWindowModelV.TutorialsType == WifiTutorialsType.RESCUE_TABLET_WIFI)
		{
			switch (selectedIndex)
			{
			case 0:
				wifiConnectHelpWindowModelV.IsPrevBtnEnable = false;
				wifiConnectHelpWindowModelV.IsNextBtnEnable = true;
				break;
			case 3:
				wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
				wifiConnectHelpWindowModelV.IsNextBtnEnable = false;
				break;
			default:
				wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
				wifiConnectHelpWindowModelV.IsNextBtnEnable = true;
				break;
			}
			switch (selectedIndex)
			{
			case 1:
				qrCode.Visibility = Visibility.Visible;
				wifiCode.Visibility = Visibility.Collapsed;
				break;
			case 3:
				qrCode.Visibility = Visibility.Collapsed;
				wifiCode.Visibility = Visibility.Visible;
				break;
			default:
				qrCode.Visibility = Visibility.Collapsed;
				wifiCode.Visibility = Visibility.Collapsed;
				break;
			}
		}
		else
		{
			switch (selectedIndex)
			{
			case 0:
				wifiConnectHelpWindowModelV.IsPrevBtnEnable = false;
				wifiConnectHelpWindowModelV.IsNextBtnEnable = true;
				break;
			case 5:
				wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
				wifiConnectHelpWindowModelV.IsNextBtnEnable = false;
				break;
			default:
				if ((wifiConnectHelpWindowModelV.TutorialsType == WifiTutorialsType.RESCUE_TABLET_DEBUG || wifiConnectHelpWindowModelV.TutorialsType == WifiTutorialsType.HWTEST) && selectedIndex == 3)
				{
					wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
					wifiConnectHelpWindowModelV.IsNextBtnEnable = false;
				}
				else
				{
					wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
					wifiConnectHelpWindowModelV.IsNextBtnEnable = true;
				}
				break;
			}
			if (m_WifiTutorialsType == WifiTutorialsType.HWTEST)
			{
				qrCode.Visibility = Visibility.Collapsed;
				wifiCode.Visibility = Visibility.Collapsed;
				switch (selectedIndex)
				{
				case 0:
					wifiConnectHelpWindowModelV.IsPrevBtnEnable = false;
					wifiConnectHelpWindowModelV.IsNextBtnEnable = true;
					break;
				case 1:
					qrCode.Visibility = Visibility.Visible;
					wifiCode.Visibility = Visibility.Collapsed;
					break;
				case 3:
					qrCode.Visibility = Visibility.Collapsed;
					wifiCode.Visibility = Visibility.Visible;
					wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
					wifiConnectHelpWindowModelV.IsNextBtnEnable = false;
					break;
				default:
					wifiConnectHelpWindowModelV.IsPrevBtnEnable = true;
					wifiConnectHelpWindowModelV.IsNextBtnEnable = true;
					break;
				}
			}
			else if (selectedIndex == 3)
			{
				qrCode.Visibility = Visibility.Visible;
				wifiCode.Visibility = Visibility.Collapsed;
			}
			else if ((!_IsHwPop && selectedIndex == 2) || (!_IsHwPop && selectedIndex == 5))
			{
				qrCode.Visibility = Visibility.Collapsed;
				wifiCode.Visibility = Visibility.Visible;
			}
			else
			{
				qrCode.Visibility = Visibility.Collapsed;
				wifiCode.Visibility = Visibility.Collapsed;
			}
		}
		if (_IsHideQrCode)
		{
			qrCode.Visibility = Visibility.Collapsed;
			wifiCode.Visibility = Visibility.Collapsed;
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
