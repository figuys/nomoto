using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class TabletEntranceViewV6 : UserControl, IComponentConnector
{
	public TabletEntranceViewV6()
	{
		InitializeComponent();
	}

	public void UpdateGridLayout()
	{
		if (MainFrameV6.Instance.IsChinaUs())
		{
			Grid.SetRow(btnFastboot, 1);
			Grid.SetRow(btnSN, 3);
			bdFastboot.Visibility = Visibility.Visible;
			btnSN.TabIndex = ((!MainFrameV6.Instance.IsChinaUs(isOnlyChina: true)) ? 2 : 0);
		}
		else
		{
			btnSN.TabIndex = 1;
			Grid.SetRow(btnSN, 1);
			Grid.SetRow(btnFastboot, 3);
			bdFastboot.Visibility = Visibility.Collapsed;
		}
	}

	private void OnBtnUSBConnection(object sender, RoutedEventArgs e)
	{
		if (!DeviceConnectedCheck())
		{
			IUserMsgControl win = new WifiConnectHelpWindowV6(isHwPop: false, "K1066")
			{
				DataContext = new WifiConnectHelpWindowModelV6(WifiTutorialsType.RESCUE_TABLET_DEBUG)
			};
			GlobalCmdHelper.Instance.TabletOpenUsbDebugCallback = delegate
			{
				UsbOpenCallback(win);
			};
			MainFrameV6.Instance.IMsgManager.ShowMessage(win);
		}
	}

	private void OnBtnWifiConnection(object sender, RoutedEventArgs e)
	{
		if (!DeviceConnectedCheck())
		{
			IUserMsgControl userUi = new WifiConnectHelpWindowV6
			{
				DataContext = new WifiConnectHelpWindowModelV6(WifiTutorialsType.RESCUE_TABLET_WIFI)
			};
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		}
	}

	private bool DeviceConnectedCheck()
	{
		DeviceEx device = HostProxy.deviceManager.ConntectedDevices.FirstOrDefault((DeviceEx n) => n.SoftStatus == DeviceSoftStateEx.Online && n.Property.Category == "tablet");
		if (device != null)
		{
			Task.Run(delegate
			{
				MainFrameV6.Instance.AutoMatch(device);
			});
			return true;
		}
		return false;
	}

	private void OnBtnInputSN(object sender, RoutedEventArgs e)
	{
		if ((sender as Button).TabIndex != 0)
		{
			MainFrameV6.Instance.ChangeView(PageIndex.TABLET_SEARCH);
		}
	}

	private void UsbOpenCallback(IUserMsgControl win)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			try
			{
				if (win != null && win is Window window)
				{
					window.Close();
					win = null;
				}
			}
			catch (Exception)
			{
			}
		});
	}
}
