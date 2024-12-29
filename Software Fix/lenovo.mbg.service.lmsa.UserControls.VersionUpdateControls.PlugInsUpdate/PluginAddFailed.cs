using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.UserControls.VersionUpdateControls.PlugInsUpdate;

public partial class PluginAddFailed : UserControl, IComponentConnector
{
	public static readonly DependencyProperty strErrorTipProperty = DependencyProperty.Register("strErrorTip", typeof(string), typeof(PluginAddFailed), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty StrErrorTipConentProperty = DependencyProperty.Register("StrErrorTipConent", typeof(string), typeof(PluginAddFailed), new PropertyMetadata(string.Empty));

	public string strErrorTip
	{
		get
		{
			return (string)GetValue(strErrorTipProperty);
		}
		set
		{
			SetValue(strErrorTipProperty, value);
		}
	}

	public string StrErrorTipConent
	{
		get
		{
			return (string)GetValue(StrErrorTipConentProperty);
		}
		set
		{
			SetValue(StrErrorTipConentProperty, value);
		}
	}

	public PluginAddFailed()
	{
		InitializeComponent();
		strErrorTip = "K0363";
		StrErrorTipConent = "K0364";
	}
}
