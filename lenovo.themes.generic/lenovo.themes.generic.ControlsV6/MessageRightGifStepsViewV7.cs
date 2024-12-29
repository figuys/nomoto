using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.Gif;

namespace lenovo.themes.generic.ControlsV6;

public partial class MessageRightGifStepsViewV7 : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action CancelCallback { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public Action<object> ConfirmCallback { get; set; }

	public MessageRightGifStepsViewV7(string _txtTitle, string _txtStep1, string _txtStep2, string _gifName, string _subTitle = "")
	{
		MessageRightGifStepsViewV7 messageRightGifStepsViewV = this;
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		txtTitle.Text = _txtTitle;
		txtTextStep1.Text = _txtStep1;
		txtTextStep2.Text = _txtStep2;
		if (string.IsNullOrEmpty(_subTitle))
		{
			txtSubTitle.Visibility = Visibility.Collapsed;
		}
		else
		{
			txtSubTitle.Visibility = Visibility.Visible;
			txtSubTitle.Text = _subTitle;
		}
		base.Loaded += delegate
		{
			BitmapImage value = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/ResourceV6/" + _gifName));
			ImageBehavior.SetAnimatedSource(messageRightGifStepsViewV.imgRightGif, value);
		};
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		CloseAction?.Invoke(Result);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	public void SetConfirmBtnStatus(bool _enabled)
	{
		btnConfirm.IsEnabled = _enabled;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Result = false;
		CancelCallback?.Invoke();
		Close();
	}

	private void BtnConfirmClick(object sender, RoutedEventArgs e)
	{
		ConfirmCallback?.Invoke(sender);
	}

	private void OnLenovoPolicyClick(object sender, MouseButtonEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}
}
