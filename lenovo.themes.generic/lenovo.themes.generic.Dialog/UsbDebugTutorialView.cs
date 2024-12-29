using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.Dialog;

public partial class UsbDebugTutorialView : Window, IComponentConnector
{
	private UsbDebugTutorialViewModel vm;

	public UsbDebugTutorialView(bool isAutoClose = false)
	{
		InitializeComponent();
		vm = new UsbDebugTutorialViewModel();
		base.DataContext = vm;
		if (!isAutoClose)
		{
			return;
		}
		btnPhone.Visibility = Visibility.Collapsed;
		btnTablet.IsChecked = true;
		vm.DevTypeCommand.Execute("False");
		GlobalCmdHelper.Instance.AutoCloseUsbConnectTutorial = delegate
		{
			base.Dispatcher.Invoke(delegate
			{
				Close();
			});
		};
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
