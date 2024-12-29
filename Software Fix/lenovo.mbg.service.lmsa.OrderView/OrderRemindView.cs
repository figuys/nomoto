using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderRemindView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public OrderRemindView(int used, int total)
	{
		InitializeComponent();
		if (used >= total)
		{
			txtTitle.Text = LangTranslation.Translate("K1853");
		}
		else
		{
			txtTitle.Text = LangTranslation.Translate("K1852");
		}
		txtInfo.Text = string.Format(LangTranslation.Translate("K1686"), used, total);
		BuyPanel.OnBuyAction = delegate
		{
			BtnClose_Click(null, null);
			ApplcationClass.ApplcationStartWindow.ShowMessage("K1730");
		};
	}

	private void BtnClose_Click(object sender, RoutedEventArgs e)
	{
		CloseAction?.Invoke(false);
		Close();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
