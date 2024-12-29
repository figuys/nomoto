using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class SetPasswordWindow : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public SetPasswordWindow()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		CloseAction(true);
	}

	public Window GetMsgUi()
	{
		return this;
	}
}
