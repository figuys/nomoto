using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class FindTabletSNView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public FindTabletSNView()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(false);
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		Plugin.OperateTracker("TabletManualSelectLinkClick", "User clicked tablet manual select link!");
		Close();
		CloseAction?.Invoke(true);
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

	public Window GetMsgUi()
	{
		return this;
	}
}
