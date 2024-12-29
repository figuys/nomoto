using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.flash.Common;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class RescueFrameV6 : UserControl, IComponentConnector
{
	public FrameworkElement View { get; private set; }

	public RescueFrameV6(AutoMatchViewModel vm)
	{
		InitializeComponent();
		base.DataContext = vm;
	}

	public void ChangeView(FlashStatusV6 statusV6, Action<FrameworkElement> viewInitAction = null)
	{
		switch (statusV6)
		{
		case FlashStatusV6.Rescuing:
			viewInitAction(rescuing);
			rescuing.Visibility = Visibility.Visible;
			View = rescuing;
			break;
		case FlashStatusV6.PASS:
			rescuing.Dispose();
			viewInitAction(success);
			success.Visibility = Visibility.Visible;
			View = success;
			break;
		default:
			rescuing.Dispose();
			viewInitAction(fail);
			fail.Visibility = Visibility.Visible;
			View = fail;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
