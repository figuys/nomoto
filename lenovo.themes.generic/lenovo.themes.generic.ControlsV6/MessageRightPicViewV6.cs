using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.themes.generic.ControlsV6;

public partial class MessageRightPicViewV6 : UserControl, IMessageViewV6, IComponentConnector
{
	private bool _IsPopup;

	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public MessageRightPicViewV6()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(ImageSource picture, string title, string message, string btnOkTitle, string btnCancelTitle, string tips = null, bool isCloseBtn = false, bool isPopup = false)
	{
		_IsPopup = isPopup;
		tbkTitle.LangKey = title;
		txtInfo.LangKey = message;
		btnOk.Visibility = (string.IsNullOrEmpty(btnOkTitle) ? Visibility.Collapsed : Visibility.Visible);
		btnOk.LangKey = btnOkTitle;
		btnCancel.Visibility = (string.IsNullOrEmpty(btnCancelTitle) ? Visibility.Collapsed : Visibility.Visible);
		btnCancel.LangKey = btnCancelTitle;
		txtTips.Visibility = (string.IsNullOrEmpty(tips) ? Visibility.Collapsed : Visibility.Visible);
		txtTips.LangKey = tips;
		pop.Visibility = ((!isPopup) ? Visibility.Collapsed : Visibility.Visible);
		btnClose.Visibility = ((!isCloseBtn) ? Visibility.Collapsed : Visibility.Visible);
		img.Source = picture;
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
		if (_IsPopup)
		{
			pop.IsOpen = true;
		}
		else
		{
			FireClose(null);
		}
	}

	private void OnBtnForce(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
		FireClose(null);
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
	}

	private void OnBtnConitue(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
