using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Control;

public partial class RescueMatchComboBox : UserControl, IComponentConnector
{
	public static readonly DependencyProperty IsSearchableProperty = DependencyProperty.Register("IsSearchable", typeof(bool), typeof(RescueMatchComboBox), new UIPropertyMetadata(true));

	[DefaultValue(true)]
	public bool IsSearchable
	{
		get
		{
			return (bool)GetValue(IsSearchableProperty);
		}
		set
		{
			SetValue(IsSearchableProperty, value);
		}
	}

	public RescueMatchComboBox()
	{
		InitializeComponent();
	}
}
