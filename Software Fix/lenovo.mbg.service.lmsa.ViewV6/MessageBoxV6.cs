using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class MessageBoxV6 : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public MessageBoxV6()
	{
		InitializeComponent();
	}

	public void Init(string title, string message, string btnOkTitle, string btnCancelTitle, bool isCloseBtn, MessageBoxImage icon, bool isPrivacy = false)
	{
		tbkTitle.LangKey = title;
		txtInfo.LangKey = message;
		if (!string.IsNullOrEmpty(btnOkTitle))
		{
			btnOk.LangKey = btnOkTitle;
		}
		else
		{
			btnOk.Visibility = Visibility.Collapsed;
		}
		if (!string.IsNullOrEmpty(btnCancelTitle))
		{
			btnCancel.LangKey = btnCancelTitle;
			btnCancel.Visibility = Visibility.Visible;
		}
		else
		{
			btnCancel.Visibility = Visibility.Collapsed;
		}
		switch (icon)
		{
		case MessageBoxImage.Exclamation:
			IconPath.Fill = TryFindResource("V6_WarningBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_WarningBkgBrushKey") as SolidColorBrush;
			break;
		case MessageBoxImage.Hand:
			IconPath.Fill = TryFindResource("V6_ErrorBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_ErrorBkgBrushKey") as SolidColorBrush;
			break;
		}
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		txtPrivacy.Visibility = ((!isPrivacy) ? Visibility.Collapsed : Visibility.Visible);
	}

	public void Init(string title, string message, string okBtnText, string cancelBtnText, MessageBoxImage icon, string notifyText)
	{
		tbkTitle.LangKey = title;
		txtInfo.LangKey = message;
		if (!string.IsNullOrEmpty(okBtnText))
		{
			btnOk.LangKey = okBtnText;
		}
		else
		{
			btnOk.Visibility = Visibility.Collapsed;
		}
		if (!string.IsNullOrEmpty(cancelBtnText))
		{
			btnCancel.LangKey = cancelBtnText;
			btnCancel.Visibility = Visibility.Visible;
		}
		else
		{
			btnCancel.Visibility = Visibility.Collapsed;
		}
		if (string.IsNullOrEmpty(notifyText))
		{
			panelNotify.Visibility = Visibility.Collapsed;
		}
		else
		{
			panelNotify.Visibility = Visibility.Visible;
			txtNotify.LangKey = notifyText;
		}
		switch (icon)
		{
		case MessageBoxImage.Exclamation:
			IconPath.Fill = TryFindResource("V6_WarningBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_WarningBkgBrushKey") as SolidColorBrush;
			break;
		case MessageBoxImage.Hand:
			IconPath.Fill = TryFindResource("V6_ErrorBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_ErrorBkgBrushKey") as SolidColorBrush;
			break;
		}
		btnClose.Visibility = Visibility.Collapsed;
		txtPrivacy.Visibility = Visibility.Collapsed;
	}

	public void Init(string message, MessageBoxButton btn, MessageBoxImage icon, bool isCloseBtn, bool isPrivacy = false)
	{
		txtInfo.LangKey = message;
		switch (btn)
		{
		case MessageBoxButton.OKCancel:
			btnOk.LangKey = "K0327";
			btnCancel.LangKey = "K0208";
			btnCancel.Visibility = Visibility.Visible;
			break;
		case MessageBoxButton.YesNo:
			btnOk.LangKey = "K0571";
			btnCancel.LangKey = "K0570";
			btnCancel.Visibility = Visibility.Visible;
			break;
		default:
			btnOk.LangKey = "K0327";
			btnCancel.Visibility = Visibility.Collapsed;
			break;
		}
		switch (icon)
		{
		case MessageBoxImage.Hand:
			tbkTitle.Text = "Error";
			IconPath.Width = 27.0;
			IconPath.Height = 24.0;
			IconPath.Data = TryFindResource("V6_IconWarning") as PathGeometry;
			IconPath.Fill = TryFindResource("V6_WarningBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_WarningBkgBrushKey") as SolidColorBrush;
			break;
		case MessageBoxImage.Exclamation:
			tbkTitle.LangKey = "K0071";
			IconPath.Width = 27.0;
			IconPath.Height = 24.0;
			IconPath.Data = TryFindResource("V6_IconWarning") as PathGeometry;
			IconPath.Fill = TryFindResource("V6_WarningBrushKey") as SolidColorBrush;
			IconBd.Background = TryFindResource("V6_WarningBkgBrushKey") as SolidColorBrush;
			break;
		default:
			tbkTitle.LangKey = "K0711";
			break;
		}
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		txtPrivacy.Visibility = ((!isPrivacy) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(false);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(true);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(null);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		Process.Start("www.lenovo.com/privacy/");
	}
}
