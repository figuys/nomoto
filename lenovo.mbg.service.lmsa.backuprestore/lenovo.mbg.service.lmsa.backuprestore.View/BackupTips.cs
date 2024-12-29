using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class BackupTips : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public BackupTips()
	{
		InitializeComponent();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(false);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(true);
	}
}
