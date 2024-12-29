using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.flash.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class PhoneMMatchViewV6 : UserControl, IComponentConnector
{
	public PhoneMMatchViewModelV6 VM { get; private set; }

	public PhoneMMatchViewV6()
	{
		InitializeComponent();
		base.Tag = PageIndex.PHONE_MANUAL;
		VM = new PhoneMMatchViewModelV6(this);
		base.DataContext = VM;
	}

	public void ChangeConfirmVisibile(Visibility visibile)
	{
		base.Dispatcher.Invoke(() => confirm.Visibility = visibile);
	}

	private void OnBtnPrevious(object sender, RoutedEventArgs e)
	{
		VM.Previous();
	}

	private void OnBtnNext(object sender, RoutedEventArgs e)
	{
		VM.Next();
	}

	private void OnRbtnChecked(object sender, RoutedEventArgs e)
	{
		if (VM != null)
		{
			RadioButton radioButton = e.OriginalSource as RadioButton;
			VM.ShowTutorial(Convert.ToBoolean(radioButton.Tag));
		}
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		MainFrameV6.Instance.ShowTutoiral(DevCategory.Phone);
	}

	private void OnBtnConfirm(object sender, RoutedEventArgs e)
	{
		VM.GotoRescueView();
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
