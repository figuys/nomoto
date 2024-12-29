using System;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.Dialog.Permissions;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class UserConsentWindow : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	protected ConfirmAppPermissionIsReadyViewModel Vm { get; }

	public UserConsentWindow(CancellationTokenSource cts)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		Vm = new ConfirmAppPermissionIsReadyViewModel
		{
			ManualColseNotifyEvent = delegate
			{
				cts.Cancel();
			}
		};
		base.DataContext = Vm;
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
