using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.UserControls.Tips;

public partial class DonotEnableDebugWindow : Window, IComponentConnector
{
	public bool CanClose { get; set; }

	public DonotEnableDebugWindow()
	{
		CanClose = false;
		InitializeComponent();
	}

	private void CloseClick(object sender, RoutedEventArgs e)
	{
		CanClose = true;
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
