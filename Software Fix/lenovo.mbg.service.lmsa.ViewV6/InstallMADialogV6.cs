using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class InstallMADialogV6 : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public InstallMADialogV6()
	{
		InitializeComponent();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void BtnSaveClick(object sender, RoutedEventArgs e)
	{
		Result = true;
		Close();
		CloseAction?.Invoke(true);
	}

	private void BtnCloseClick(object sender, RoutedEventArgs e)
	{
		Result = false;
		Close();
		CloseAction?.Invoke(false);
	}
}
