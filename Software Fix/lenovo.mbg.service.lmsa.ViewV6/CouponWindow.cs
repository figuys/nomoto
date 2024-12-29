using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class CouponWindow : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public CouponWindow()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	public Window GetMsgUi()
	{
		return this;
	}
}
