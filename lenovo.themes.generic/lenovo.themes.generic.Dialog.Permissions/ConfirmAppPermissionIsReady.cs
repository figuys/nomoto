using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.Dialog.Permissions;

public partial class ConfirmAppPermissionIsReady : UserControl, IComponentConnector
{
	public ConfirmAppPermissionIsReady()
	{
		InitializeComponent();
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		Window.GetWindow(sender as Button)?.Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
