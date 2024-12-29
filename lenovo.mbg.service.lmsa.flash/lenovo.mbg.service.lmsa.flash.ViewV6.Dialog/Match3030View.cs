using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class Match3030View : Window, IUserMsgControl, IComponentConnector
{
	public Match3030ViewModel VM { get; private set; }

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public Match3030View(DeviceEx device, RescueDeviceInfoModel deviceInfo, FrameworkElement parentView, ResourceResponseModel response, string matchText, DevCategory category, BusinessData businessData, object wModel)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		VM = new Match3030ViewModel(this, device, deviceInfo, parentView, response, matchText, category, businessData, wModel);
		base.DataContext = VM;
	}

	public void CloseWnd(bool? isOk)
	{
		Result = Convert.ToBoolean(isOk);
		Close();
		CloseAction?.Invoke(isOk);
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		bool flag = Convert.ToBoolean((sender as Button).Tag);
		CloseWnd(flag);
		if (flag)
		{
			VM.OnConfirmMatched();
		}
	}

	public Window GetMsgUi()
	{
		return this;
	}

	public static void ProcMatch3030(DeviceEx device, RescueDeviceInfoModel deviceInfo, FrameworkElement parentView, ResourceResponseModel response, string matchText, DevCategory category, BusinessData businessData, object wModel)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			Match3030View userUi = new Match3030View(device, deviceInfo, parentView, response, matchText, category, businessData, wModel);
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		});
	}
}
