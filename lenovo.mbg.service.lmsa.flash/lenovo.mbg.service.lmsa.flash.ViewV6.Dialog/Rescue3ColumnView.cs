using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class Rescue3ColumnView : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public Rescue3ColumnView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		ColumnViewModel obj = base.DataContext as ColumnViewModel;
		if (obj != null && !obj.IsClosePopup)
		{
			OnPopCancel(null, null);
		}
		else
		{
			pop.IsOpen = true;
		}
	}

	private void OnPopCancel(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
		Result = null;
		WaitHandler.Set();
		CloseAction?.Invoke(null);
	}

	private void OnPopOk(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
	}
}
