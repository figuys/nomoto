using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.flash.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class SmartMMatchViewV6 : UserControl, IComponentConnector
{
	public SmartMMatchViewModel VM { get; set; }

	public SmartMMatchViewV6()
	{
		InitializeComponent();
		base.Tag = PageIndex.SMART_MANUAL;
		VM = new SmartMMatchViewModel(this, DevCategory.Smart);
		base.DataContext = VM;
	}

	public void ChangeConfirmVisibile(Visibility visibile)
	{
		base.Dispatcher.Invoke(() => confirm.Visibility = visibile);
	}

	private void OnBtnConfirm(object sender, RoutedEventArgs e)
	{
		VM.GotoRescueView();
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		MainFrameV6.Instance.ShowTutoiral(DevCategory.Smart);
	}

	private void OnReselectRom(object sender, MouseButtonEventArgs e)
	{
		VM.ReSelectRom();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
