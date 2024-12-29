using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace lenovo.mbg.service.common.utilities;

public class GlobalCmdHelper : ICommand
{
	private static GlobalCmdHelper instance;

	public Action AutoCloseUsbConnectTutorial;

	public Action CloseRescueDlgEvent;

	public Action CloseGuidStepDlgEvent;

	public Action CloseRescueHelpEvent;

	public Action CloseWifiTutorialEvent;

	public Action<string> OnDelRomAfterRescue;

	public Action<List<string>> OnDelRomAfterRescueRetry;

	public Action<bool> ReadDevieInfoCallback;

	public Action TabletOpenUsbDebugCallback;

	public static GlobalCmdHelper Instance => instance ?? (instance = new GlobalCmdHelper());

	public event Action<bool> SwitchDeviceEventHander;

	public event EventHandler CanExecuteChanged
	{
		add
		{
			CommandManager.RequerySuggested += value;
		}
		remove
		{
			CommandManager.RequerySuggested -= value;
		}
	}

	public bool CanExecute(object parameter)
	{
		return true;
	}

	public void Execute(dynamic param)
	{
		if (param.type == GlobalCmdType.SWITCHDEVICE)
		{
			if (this.SwitchDeviceEventHander != null)
			{
				this.SwitchDeviceEventHander(param.data);
			}
		}
		else if (param.type == GlobalCmdType.AUTO_CLOSE_USB_CONN_TUTORIAL)
		{
			AutoCloseUsbConnectTutorial?.Invoke();
		}
		else if (param.type == GlobalCmdType.CLOSE_RESCUE_DIALOG)
		{
			CloseRescueDlgEvent?.Invoke();
		}
		else if (param.type == GlobalCmdType.DELETE_ROM_AFTER_RESCUE)
		{
			OnDelRomAfterRescue?.Invoke(param.data);
		}
		else if (param.type == GlobalCmdType.DELETE_ROM_AFTER_RESCUE_RETRY)
		{
			OnDelRomAfterRescueRetry?.Invoke(param.data);
		}
		else if (param.type == GlobalCmdType.CLOSE_GUID_SETP_DIALOG)
		{
			CloseGuidStepDlgEvent?.Invoke();
		}
		else if (param.type == GlobalCmdType.CLOSE_RESCUE_HELP)
		{
			CloseRescueHelpEvent?.Invoke();
		}
		else if (param.type == GlobalCmdType.CLOSE_WIFI_TUTORIAL)
		{
			CloseWifiTutorialEvent?.Invoke();
		}
		else if (param.type == GlobalCmdType.READ_DEVICEINFO_CALLBACK)
		{
			ReadDevieInfoCallback?.Invoke(param.data);
		}
		else if (param.type == GlobalCmdType.TABLET_OPEN_USBDEBUG)
		{
			TabletOpenUsbDebugCallback?.Invoke();
		}
	}
}
