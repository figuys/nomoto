using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa;

public partial class ProgressBar_Common : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register("ProgressBarValue", typeof(double), typeof(ProgressBar_Common), new PropertyMetadata(0.0));

	public double ProgressBarValue
	{
		get
		{
			return (double)GetValue(ProgressBarValueProperty);
		}
		set
		{
			SetValue(ProgressBarValueProperty, value);
		}
	}

	public ProgressBar_Common()
	{
		InitializeComponent();
	}
}
