using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class SMSDetailView : UserControl, IComponentConnector
{
	public SMSDetailView()
	{
		InitializeComponent();
	}

	private void BtnClose_Click(object sender, RoutedEventArgs e)
	{
		base.Visibility = Visibility.Hidden;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
