using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class PhoneEntranceViewV6 : UserControl, IComponentConnector
{
	public PhoneEntranceViewV6()
	{
		InitializeComponent();
	}

	public void UpdateGridLayout()
	{
		if (MainFrameV6.Instance.IsChinaUs())
		{
			Grid.SetRow(btnFastboot, 1);
			Grid.SetRow(btnImei, 3);
			bdFastboot.Visibility = Visibility.Visible;
			btnImei.TabIndex = ((!MainFrameV6.Instance.IsChinaUs(isOnlyChina: true)) ? 2 : 0);
		}
		else
		{
			btnImei.TabIndex = 1;
			Grid.SetRow(btnImei, 1);
			Grid.SetRow(btnFastboot, 3);
			bdFastboot.Visibility = Visibility.Collapsed;
		}
	}

	private void OnBtnFastboot(object sender, RoutedEventArgs e)
	{
		Task.Factory.StartNew(delegate
		{
			DriversHelper.CheckAndInstallInfDriver(DriverType.Motorola, null, out var _);
			DriversHelper.CheckMotorolaDriverExeInstalled(delegate(string _arg)
			{
				MainFrameV6.Instance.IMsgManager.SetDriverButtonStatus(_arg);
			});
		});
		base.Dispatcher.BeginInvoke((Action)delegate
		{
			MainFrameV6.Instance.ShowGifGuideSteps(_showTextDetect: true, null);
		});
	}

	private void OnBtnWifiConnection(object sender, RoutedEventArgs e)
	{
		IUserMsgControl userUi = new WifiConnectHelpWindowV6
		{
			DataContext = new WifiConnectHelpWindowModelV6()
		};
		MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
	}

	private void OnBtnInputImei(object sender, RoutedEventArgs e)
	{
		if ((sender as Button).TabIndex != 0)
		{
			MainFrameV6.Instance.ChangeView(PageIndex.PHONE_SEARCH);
		}
	}

	private void OnBtnManualSelection(object sender, RoutedEventArgs e)
	{
		MainFrameV6.Instance.ChangeView(PageIndex.PHONE_MANUAL);
	}
}
