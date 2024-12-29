using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.Dialog.Permissions;

public partial class NormalPermissionsTipView : UserControl, IComponentConnector
{
	public static readonly DependencyProperty TipContentProperty = DependencyProperty.Register("TipContent", typeof(string), typeof(NormalPermissionsTipView), new PropertyMetadata("K0769"));

	public static readonly DependencyProperty PermissionItemTipProperty = DependencyProperty.Register("PermissionItemTip", typeof(string), typeof(NormalPermissionsTipView), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty ConnectedAppTypeProperty = DependencyProperty.Register("ConnectedAppType", typeof(string), typeof(NormalPermissionsTipView), new PropertyMetadata(null, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		string value = e.NewValue?.ToString() ?? string.Empty;
		if ("Ma".Equals(value))
		{
			string value2 = "K0750";
			d.SetValue(PermissionItemTipProperty, value2);
		}
		else if ("Moto".Equals(value))
		{
			string value3 = "K0797";
			d.SetValue(PermissionItemTipProperty, value3);
		}
		else
		{
			d.SetValue(PermissionItemTipProperty, string.Empty);
		}
	}));

	public string TipContent
	{
		get
		{
			return (string)GetValue(TipContentProperty);
		}
		set
		{
			SetValue(TipContentProperty, value);
		}
	}

	public string PermissionItemTip
	{
		get
		{
			return (string)GetValue(PermissionItemTipProperty);
		}
		set
		{
			SetValue(PermissionItemTipProperty, value);
		}
	}

	public string ConnectedAppType
	{
		get
		{
			return (string)GetValue(ConnectedAppTypeProperty);
		}
		set
		{
			SetValue(ConnectedAppTypeProperty, value);
		}
	}

	public NormalPermissionsTipView()
	{
		InitializeComponent();
	}
}
