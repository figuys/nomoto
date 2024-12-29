using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.themes.generic.ControlsV6;

public partial class MessageCommViewV6 : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public MessageCommViewV6()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(string message, MessageBoxButton btn, MessageBoxImage icon, bool isCloseBtn, bool isPrivacy = false)
	{
		string btnOkTitle = "K0327";
		string btnCancelTitle = null;
		switch (btn)
		{
		case MessageBoxButton.OKCancel:
			btnCancelTitle = "K0208";
			break;
		case MessageBoxButton.YesNo:
			btnOkTitle = "K0571";
			btnCancelTitle = "K0570";
			break;
		}
		Init(null, message, btnOkTitle, btnCancelTitle, isCloseBtn, icon, null, isPrivacy);
	}

	public void Init(string title, string message, string btnOkTitle, string btnCancelTitle, bool isCloseBtn, MessageBoxImage icon, string notifyText = null, bool isPrivacy = false)
	{
		txtInfo.LangKey = message;
		tbkTitle.LangKey = title;
		switch (icon)
		{
		case MessageBoxImage.Exclamation:
			tbkTitle.LangKey = title ?? "K0071";
			IconPath.Width = 27.0;
			IconPath.Height = 24.0;
			IconPath.Data = TryFindResource("V6_IconWarning") as PathGeometry;
			IconPath.Fill = TryFindResource("V6_WarningBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_WarningBkgBrushKey") as SolidColorBrush;
			break;
		case MessageBoxImage.Hand:
			tbkTitle.LangKey = title ?? "Error";
			IconPath.Fill = TryFindResource("V6_ErrorBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_ErrorBkgBrushKey") as SolidColorBrush;
			break;
		default:
			tbkTitle.LangKey = title ?? "K0711";
			break;
		}
		btnOk.LangKey = btnOkTitle;
		btnOk.Visibility = (string.IsNullOrEmpty(btnOkTitle) ? Visibility.Collapsed : Visibility.Visible);
		btnCancel.LangKey = btnCancelTitle;
		btnCancel.Visibility = (string.IsNullOrEmpty(btnCancelTitle) ? Visibility.Collapsed : Visibility.Visible);
		txtNotify.LangKey = notifyText;
		panelNotify.Visibility = (string.IsNullOrEmpty(notifyText) ? Visibility.Collapsed : Visibility.Visible);
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		txtPrivacy.Visibility = ((!isPrivacy) ? Visibility.Collapsed : Visibility.Visible);
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

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		Process.Start("www.lenovo.com/privacy/");
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
	}
}
