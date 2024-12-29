using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class PhoneEnterySelView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public PhoneEnterySelView()
	{
		InitializeComponent();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	public static bool? ShowEx()
	{
		PhoneEnterySelView userUi = new PhoneEnterySelView();
		return MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		Result = false;
		CloseAction?.Invoke(false);
	}

	private void OnRbtnChecked(object sender, RoutedEventArgs e)
	{
		OnBtnClose(null, null);
		RadioButton radioButton = sender as RadioButton;
		if (radioButton.Name == "rbtn1")
		{
			IUserMsgControl userUi = new WifiConnectHelpWindowV6
			{
				DataContext = new WifiConnectHelpWindowModelV6()
			};
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		}
		else if (radioButton.Name == "rbtn2")
		{
			Plugin.OperateTracker("ManuallyEntertheIMEI", "User clicked manually enter the imei");
			FindIMEIView findIMEIView = new FindIMEIView();
			MainFrameV6.Instance.IMsgManager.ShowMessage(findIMEIView);
			if (findIMEIView.Result == true)
			{
				MainFrameV6.Instance.ShowGifGuideSteps(_showTextDetect: true, null);
			}
			else
			{
				MainFrameV6.Instance.ChangeView(PageIndex.PHONE_SEARCH);
			}
		}
		else if (radioButton.Name == "rbtn3")
		{
			Plugin.OperateTracker("Select your model", "User clicked Select your model");
			MainFrameV6.Instance.ChangeView(PageIndex.PHONE_MANUAL);
		}
	}
}
