using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescuingView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public RescuingView()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	public Window GetMsgUi()
	{
		return this;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		CloseAction?.Invoke(true);
	}
}
