using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class PicMessageBox : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public PicMessageBox()
	{
		InitializeComponent();
	}

	public void Init(string title, string message, string imagePath, string btnOkTitle, string btnCancelTitle, bool isCloseBtn, bool isNotifyText)
	{
		if (!string.IsNullOrEmpty(title) && Regex.IsMatch(title, "^K\\d{4}"))
		{
			tbkTitle.LangKey = title;
		}
		else
		{
			tbkTitle.Text = title;
		}
		if (!string.IsNullOrEmpty(message) && Regex.IsMatch(message, "^K\\d{4}"))
		{
			txtInfo.LangKey = message;
		}
		else
		{
			txtInfo.Text = message;
		}
		if (!string.IsNullOrEmpty(btnOkTitle))
		{
			if (Regex.IsMatch(btnOkTitle, "^K\\d{4}"))
			{
				btnOk.LangKey = btnOkTitle;
			}
			else
			{
				btnOk.Content = btnOkTitle;
			}
			btnOk.Visibility = Visibility.Visible;
		}
		else
		{
			btnOk.Visibility = Visibility.Collapsed;
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
		panelNotify.Visibility = ((!isNotifyText) ? Visibility.Collapsed : Visibility.Visible);
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		if (!string.IsNullOrEmpty(imagePath))
		{
			img.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
		}
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
}
