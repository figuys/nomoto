using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class FindSmartSNView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public FindSmartSNView()
	{
		InitializeComponent();
	}

	private void OnBtnBack(object sender, RoutedEventArgs e)
	{
		PanelPrev.Visibility = Visibility.Visible;
		btnNext.Visibility = Visibility.Visible;
		btnPrev.Visibility = Visibility.Collapsed;
		PanelNext.Visibility = Visibility.Collapsed;
	}

	private void OnBtnNext(object sender, RoutedEventArgs e)
	{
		PanelPrev.Visibility = Visibility.Collapsed;
		btnNext.Visibility = Visibility.Collapsed;
		btnPrev.Visibility = Visibility.Visible;
		PanelNext.Visibility = Visibility.Visible;
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
