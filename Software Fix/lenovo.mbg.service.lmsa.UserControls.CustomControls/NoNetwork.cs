using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.UserControls.CustomControls;

public partial class NoNetwork : UserControl, IComponentConnector
{
	public static readonly DependencyProperty StrTitleTipProperty = DependencyProperty.Register("StrTitleTip", typeof(string), typeof(NoNetwork), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty StrTipsProperty = DependencyProperty.Register("StrTips", typeof(string), typeof(NoNetwork), new PropertyMetadata(string.Empty));

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

	public NoNetwork()
	{
		InitializeComponent();
		StrTitleTip = "K0359";
		StrTips = "K0358";
	}
}
