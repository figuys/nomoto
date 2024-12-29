using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace lenovo.themes.generic.Controls.Windows;

public partial class WaitTips : Window, IComponentConnector
{
	public WaitTips(string msgKey)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		info.LangKey = msgKey;
	}

	public void SetInfoText(string msgKey)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			info.LangKey = msgKey;
		});
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
