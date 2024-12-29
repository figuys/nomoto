using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class MessageWithLinkerView : UserControl, IMessageViewV6, IComponentConnector, IStyleConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public MessageWithLinkerView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(string title, string message, string btnOkTitle, string btnCancelTitle, List<Tuple<string, string>> LinkerArr, bool isCloseBtn = false)
	{
		tbkTitle.LangKey = title;
		txtInfo.LangKey = message;
		LinkList.ItemsSource = LinkerArr;
		btnOk.Visibility = (string.IsNullOrEmpty(btnOkTitle) ? Visibility.Collapsed : Visibility.Visible);
		btnOk.LangKey = btnOkTitle;
		btnCancel.Visibility = (string.IsNullOrEmpty(btnCancelTitle) ? Visibility.Collapsed : Visibility.Visible);
		btnCancel.LangKey = btnCancelTitle;
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void OnLBtnDown(object sender, MouseButtonEventArgs e)
	{
		string text = (sender as TextBlock)?.Tag as string;
		if (!string.IsNullOrEmpty(text))
		{
			Process.Start(text);
		}
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
