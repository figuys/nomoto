using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.ControlsV6;

public partial class SdCardView : UserControl, IComponentConnector
{
	public static readonly DependencyProperty TitleMaxWidthProperty = DependencyProperty.Register("TitleMaxWidth", typeof(double), typeof(SdCardView), new PropertyMetadata(9999.0));

	public ComboBox Cbx => GetTemplateChild("combobox") as ComboBox;

	public double TitleMaxWidth
	{
		get
		{
			return (double)GetValue(TitleMaxWidthProperty);
		}
		set
		{
			SetValue(TitleMaxWidthProperty, value);
		}
	}

	public SdCardView()
	{
		InitializeComponent();
	}
}
