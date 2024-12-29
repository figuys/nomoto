using System;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class SMSAppletSettingTips
{
	public static void DoProcess(Action<Action, Action> action)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (currentDevice == null)
		{
			return;
		}
		int apiLevel = currentDevice.Property.ApiLevel;
		DeviceSmsManagement mgt = new DeviceSmsManagement();
		bool needSetSmsApplet = mgt.IsNeedSetSMSApplet();
		action(delegate
		{
			if (needSetSmsApplet && !mgt.SmsAppletIsReady(currentDevice))
			{
				Window win = ((apiLevel >= 29) ? ((Window)new SmsAppletIsReadyWindowForAndroidQ(currentDevice)) : ((Window)new SmsAppletIsReadyWindow(currentDevice)));
				HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
				{
					win.ShowDialog();
				});
			}
		}, delegate
		{
			if (needSetSmsApplet)
			{
				Task.Factory.StartNew(delegate
				{
					mgt.ResetSmsAppletToDefault(currentDevice);
				});
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					Window resetTisp = ((apiLevel >= 29) ? ((Window)new ResetToDefaultSmsAppletTipsForAndroidQ(currentDevice)) : ((Window)new ResetToDefaultSmsAppletTips()));
					HostProxy.HostMaskLayerWrapper.New(resetTisp, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
					{
						resetTisp.ShowDialog();
					});
				});
			}
		});
	}
}
