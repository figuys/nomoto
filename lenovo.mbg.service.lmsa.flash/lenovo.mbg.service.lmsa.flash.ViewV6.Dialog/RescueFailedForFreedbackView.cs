using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueFailedForFreedbackView : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public RescueFailedForFreedbackView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	private void OnFeedback(object sender, MouseButtonEventArgs e)
	{
		HostProxy.HostOperationService.ShowFeedBack();
		FireClose(true);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		FireClose(null);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		FireClose(true);
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		FireClose(false);
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
	}
}
