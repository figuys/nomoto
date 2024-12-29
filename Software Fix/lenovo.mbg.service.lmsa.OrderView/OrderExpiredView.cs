using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderExpiredView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public OrderExpiredView(int _modeType)
	{
		InitializeComponent();
		BuyPanel.OnBuyAction = delegate
		{
			BtnClose_Click(null, null);
			ApplcationClass.ApplcationStartWindow.ShowMessage("Rescue the phone only at the current Mac address after successful purchase.");
		};
		if (_modeType == 2)
		{
			txtTiltle.Text = LangTranslation.Translate("K1659");
		}
		else
		{
			txtTiltle.Text = LangTranslation.Translate("K1658");
		}
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
