using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class ContentMessageBox : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public ContentMessageBox(FrameworkElement ui, string title, string btnOkTitle, string btnCancelTitle = null, bool isCloseBtn = false, MessageBoxImage icon = MessageBoxImage.Asterisk)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		content.Content = ui;
		ui.Tag = this;
		if (!string.IsNullOrEmpty(title) && Regex.IsMatch(title, "^K\\d{4}"))
		{
			tbkTitle.LangKey = title;
		}
		else
		{
			tbkTitle.Text = title;
		}
		if (!string.IsNullOrEmpty(btnOkTitle) && Regex.IsMatch(btnOkTitle, "^K\\d{4}"))
		{
			btnOk.LangKey = btnOkTitle;
		}
		else
		{
			btnOk.Content = btnOkTitle;
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
		if (!string.IsNullOrEmpty(btnCancelTitle))
		{
			if (Regex.IsMatch(btnCancelTitle, "^K\\d{4}"))
			{
				btnCancel.LangKey = btnCancelTitle;
			}
			else
			{
				btnCancel.Content = btnCancelTitle;
			}
			btnCancel.Visibility = Visibility.Visible;
		}
		else
		{
			btnCancel.Visibility = Visibility.Collapsed;
		}
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		Result = null;
		CloseAction?.Invoke(null);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		Close();
		Result = true;
		CloseAction?.Invoke(true);
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		Close();
		Result = false;
		CloseAction?.Invoke(false);
	}
}
