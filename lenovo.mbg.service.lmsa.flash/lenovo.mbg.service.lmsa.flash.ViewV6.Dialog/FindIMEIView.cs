using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class FindIMEIView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public FindIMEIView()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	private void OnBtnBack(object sender, RoutedEventArgs e)
	{
		moreImg.Visibility = Visibility.Visible;
		morePanel.Visibility = Visibility.Visible;
		backImg.Visibility = Visibility.Collapsed;
		backPanel.Visibility = Visibility.Collapsed;
	}

	private void OnBtnNext(object sender, RoutedEventArgs e)
	{
		moreImg.Visibility = Visibility.Collapsed;
		morePanel.Visibility = Visibility.Collapsed;
		backImg.Visibility = Visibility.Visible;
		backPanel.Visibility = Visibility.Visible;
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		Close();
		Result = true;
		CloseAction?.Invoke(true);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		Result = false;
		CloseAction?.Invoke(false);
	}

	public Window GetMsgUi()
	{
		return this;
	}
}
