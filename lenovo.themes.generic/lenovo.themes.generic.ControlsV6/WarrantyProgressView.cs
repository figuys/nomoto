using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.ControlsV6;

public partial class WarrantyProgressView : UserControl, IComponentConnector
{
	public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(double), typeof(WarrantyProgressView), new PropertyMetadata(0.0));

	public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register("EndValue", typeof(double), typeof(WarrantyProgressView), new PropertyMetadata(0.0));

	public double StartValue
	{
		get
		{
			return (double)GetValue(StartValueProperty);
		}
		set
		{
			SetValue(StartValueProperty, value);
		}
	}

	public double EndValue
	{
		get
		{
			return (double)GetValue(EndValueProperty);
		}
		set
		{
			SetValue(EndValueProperty, value);
		}
	}

	public WarrantyProgressView()
	{
		InitializeComponent();
	}
}
