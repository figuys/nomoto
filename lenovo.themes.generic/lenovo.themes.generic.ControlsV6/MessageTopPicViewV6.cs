using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.themes.generic.ControlsV6;

public partial class MessageTopPicViewV6 : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public MessageTopPicViewV6()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(string title, string message, ImageSource image, string btnOkTitle, string btnCancelTitle, bool isCloseBtn, bool isNotifyText)
	{
		tbkTitle.LangKey = title;
		txtInfo.LangKey = message;
		btnOk.LangKey = btnOkTitle;
		btnOk.Visibility = (string.IsNullOrEmpty(btnOkTitle) ? Visibility.Collapsed : Visibility.Visible);
		btnCancel.LangKey = btnCancelTitle;
		btnCancel.Visibility = (string.IsNullOrEmpty(btnCancelTitle) ? Visibility.Collapsed : Visibility.Visible);
		panelNotify.Visibility = ((!isNotifyText) ? Visibility.Collapsed : Visibility.Visible);
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		img.Source = image;
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
