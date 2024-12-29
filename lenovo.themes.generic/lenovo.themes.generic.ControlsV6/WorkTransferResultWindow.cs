using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.ControlsV6;

public partial class WorkTransferResultWindow : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public WorkTransferResultWindow()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	public void CloseCallback(bool? status)
	{
		CloseAction?.Invoke(status);
	}

	protected override void OnClosed(EventArgs e)
	{
		CloseAction?.Invoke(false);
		base.OnClosed(e);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
