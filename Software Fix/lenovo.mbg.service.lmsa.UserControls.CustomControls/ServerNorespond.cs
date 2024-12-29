using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.UserControls.CustomControls;

public partial class ServerNorespond : UserControl, IComponentConnector
{
	public static readonly DependencyProperty StrTitleTipProperty = DependencyProperty.Register("StrTitleTip", typeof(string), typeof(ServerNorespond), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty StrTipsProperty = DependencyProperty.Register("StrTips", typeof(string), typeof(ServerNorespond), new PropertyMetadata(string.Empty));

	public string StrTitleTip
	{
		get
		{
			return (string)GetValue(StrTitleTipProperty);
		}
		set
		{
			SetValue(StrTitleTipProperty, value);
		}
	}

	public string StrTips
	{
		get
		{
			return (string)GetValue(StrTipsProperty);
		}
		set
		{
			SetValue(StrTipsProperty, value);
		}
	}

	public ServerNorespond()
	{
		InitializeComponent();
		StrTitleTip = "K0375";
		StrTips = "K0376";
	}

	private void ReConnectServer(object sender, EventArgs e)
	{
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
