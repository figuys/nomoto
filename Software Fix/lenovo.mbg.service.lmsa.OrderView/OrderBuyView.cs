using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.Login.Business;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderBuyView : Window, IComponentConnector
{
	public OrderBuyView()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		BuyPanel.OnBuyAction = delegate
		{
			Close();
		};
		Bd.Visibility = ((!UserService.Single.CurrentLoggedInUser.B2BEntranceEnable) ? Visibility.Hidden : Visibility.Visible);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void OnBtnQuestion(object sender, RoutedEventArgs e)
	{
		Close();
		ApplcationClass.ApplcationStartWindow.ShowB2BPurchaseOverview();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
