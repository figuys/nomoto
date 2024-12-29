using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ControlsV6;

namespace lenovo.mbg.service.lmsa.backuprestore.Business;

public class DeviceSmsManagement
{
	public bool IsNeedSetSMSApplet()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: not null } tcpAndroidDevice))
		{
			return false;
		}
		return tcpAndroidDevice.Property.ApiLevel > 19;
	}

	public bool SmsAppletIsReady(TcpAndroidDevice device)
	{
		if (device == null)
		{
			return false;
		}
		if (device.ConnectType == ConnectType.Adb && device.Property.ApiLevel >= 29 && device != null)
		{
			((AdbDeviceEx)device).FocuseApp();
		}
		using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		try
		{
			List<PropItem> receiveData = null;
			long sequence = HostProxy.Sequence.New();
			if (messageReaderAndWriter.SendAndReceiveSync<object, PropItem>("smsAppletIsReady", "smsAppletIsReadyResponse", null, sequence, out receiveData) && receiveData != null)
			{
				return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
			return false;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("get sms contact info failed", exception);
			return false;
		}
	}

	public bool ResetSmsAppletToDefault(TcpAndroidDevice device)
	{
		if (device == null)
		{
			return false;
		}
		if (device.ConnectType == ConnectType.Adb && device.Property.ApiLevel >= 29)
		{
			((AdbDeviceEx)device).FocuseApp();
		}
		else
		{
			Thread.Sleep(1000);
		}
		using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		try
		{
			List<PropItem> receiveData = null;
			if (device.Property.ApiLevel >= 29 && device.ConnectType == ConnectType.Adb)
			{
				device.LockSoftStatus(autoRelease: true, DeviceSoftStateEx.Online);
			}
			long sequence = HostProxy.Sequence.New();
			if (messageReaderAndWriter.SendAndReceiveSync<object, PropItem>("resetSmsAppSetting", "resetSmsAppSettingResponse", null, sequence, out receiveData) && receiveData != null)
			{
				return receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
			return false;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("Reset sms applet to default failed", exception);
			return false;
		}
	}

	private bool? SetReadyAppletWnd(TcpAndroidDevice device, int level)
	{
		return Application.Current.Dispatcher.Invoke(delegate
		{
			DefaultSmsAppAuthorizeWindow defaultSmsAppAuthorizeWindow = new DefaultSmsAppAuthorizeWindow((level >= 29) ? 1 : 3)
			{
				OnCheckSMSAuthorize = delegate
				{
					TcpAndroidDevice tcpAndroidDevice = device;
					return (tcpAndroidDevice == null || tcpAndroidDevice.SoftStatus != DeviceSoftStateEx.Online) ? null : new bool?(SmsAppletIsReady(device));
				}
			};
			Context.MessageBox.ShowMessage(defaultSmsAppAuthorizeWindow);
			return defaultSmsAppAuthorizeWindow.Result;
		});
	}

	private void ResetReadyAppletWnd(TcpAndroidDevice device, int level)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (level >= 29)
			{
				MessageRightGifStepsViewV7 checkSmsPermissionView = new MessageRightGifStepsViewV7(LangTranslation.Translate("K1886"), LangTranslation.Translate("K1891"), LangTranslation.Translate("K1604"), "app_permissions_sms.gif", LangTranslation.Translate("K1601"));
				checkSmsPermissionView.ConfirmCallback = delegate
				{
					checkSmsPermissionView.Result = true;
					checkSmsPermissionView.Close();
				};
				Context.MessageBox.ShowMessage(checkSmsPermissionView);
			}
			else
			{
				IUserMsgControl userUi = new DefaultSmsAppAuthorizeWindow(2);
				Context.MessageBox.ShowMessage(userUi);
			}
		});
	}

	public void DoProcessWithChangeSMSDefault(TcpAndroidDevice device, Action action)
	{
		bool num = IsNeedSetSMSApplet();
		int apiLevel = device.Property.ApiLevel;
		Action action2 = delegate
		{
			try
			{
				action?.Invoke();
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Debug($"SMS restore failed, {arg}");
			}
		};
		if (num)
		{
			if (!SmsAppletIsReady(device) && !SetReadyAppletWnd(device, apiLevel).HasValue)
			{
				action2();
				return;
			}
			action2();
			if (ResetSmsAppletToDefault(device))
			{
				Task.Factory.StartNew(delegate
				{
					ResetReadyAppletWnd(device, apiLevel);
				});
			}
		}
		else
		{
			action2();
		}
	}
}
