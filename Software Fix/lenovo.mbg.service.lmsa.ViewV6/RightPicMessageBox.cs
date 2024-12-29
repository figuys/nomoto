using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class RightPicMessageBox : Window, IUserMsgControl, IComponentConnector
{
	private bool _IsPopup;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public RightPicMessageBox()
	{
		InitializeComponent();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	public void Init(ImageSource picture, string title, string message, string btnOkTitle, string btnCancelTitle, string tips = null, bool isCloseBtn = false, bool isPopup = false)
	{
		_IsPopup = isPopup;
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
		if (!string.IsNullOrEmpty(tips))
		{
			if (Regex.IsMatch(tips, "^K\\d{4}"))
			{
				txtTips.LangKey = tips;
			}
			else
			{
				txtTips.Text = tips;
			}
			txtTips.Visibility = Visibility.Visible;
		}
		else
		{
			txtTips.Visibility = Visibility.Collapsed;
		}
		if (!isPopup)
		{
			pop.Visibility = Visibility.Collapsed;
		}
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		img.Source = picture;
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
		if (_IsPopup)
		{
			pop.IsOpen = true;
			return;
		}
		Close();
		CloseAction?.Invoke(null);
	}

	private void OnBtnForce(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
		Close();
		CloseAction?.Invoke(null);
	}

	private void OnBtnConitue(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
	}
}
