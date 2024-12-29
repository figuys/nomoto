using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace lenovo.themes.generic.Controls.Windows;

public partial class GifDisplayWindow : Window, IComponentConnector
{
	public GifDisplayWindowViewModel VM { get; private set; }

	public GifDisplayWindow()
	{
		InitializeComponent();
		VM = new GifDisplayWindowViewModel(this);
		base.DataContext = VM;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
