using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class HomeView : UserControl, IComponentConnector
{
	public HomeView()
	{
		InitializeComponent();
	}

	private void OnSelectedChanged(object sender, SelectionChangedEventArgs e)
	{
		(e.OriginalSource as ComboBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
