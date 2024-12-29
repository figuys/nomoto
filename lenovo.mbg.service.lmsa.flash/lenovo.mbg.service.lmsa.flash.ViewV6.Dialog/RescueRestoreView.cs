using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueRestoreView : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public RescueRestoreView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		FireClose(false);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		FireClose(true);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		FireClose(null);
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
	}
}
