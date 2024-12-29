using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class LabelProgressBar : ProgressBar
{
	public static readonly DependencyProperty LabelContentBackgroundProperty = DependencyProperty.Register("LabelContentBackground", typeof(Brush), typeof(LabelProgressBar), new PropertyMetadata(null));

	public static readonly DependencyProperty LabelTitleBackgroundProperty = DependencyProperty.Register("LabelTitleBackground", typeof(Brush), typeof(LabelProgressBar), new PropertyMetadata(null));

	public static readonly DependencyProperty LabelTitlePanelPaddingProperty = DependencyProperty.Register("LabelTitlePanelPadding", typeof(Thickness), typeof(LabelProgressBar), new PropertyMetadata(default(Thickness)));

	public static readonly DependencyProperty LabelContentPanelPaddingProperty = DependencyProperty.Register("LabelContentPanelPadding", typeof(Thickness), typeof(LabelProgressBar), new PropertyMetadata(default(Thickness)));

	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LabelProgressBar), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty LableTitlePanelWidthProperty = DependencyProperty.Register("LableTitlePanelWidth", typeof(double), typeof(LabelProgressBar), new PropertyMetadata(0.0));

	public static readonly DependencyProperty BarMinValueProperty = DependencyProperty.Register("BarMinValue", typeof(double), typeof(LabelProgressBar), new PropertyMetadata(0.0));

	public static readonly DependencyProperty BarMaxValueProperty = DependencyProperty.Register("BarMaxValue", typeof(double), typeof(LabelProgressBar), new PropertyMetadata(0.0));

	public static readonly DependencyProperty BarValueProperty = DependencyProperty.Register("BarValue", typeof(double), typeof(LabelProgressBar), new PropertyMetadata(0.0));

	public Brush LabelContentBackground
	{
		get
		{
			return (Brush)GetValue(LabelContentBackgroundProperty);
		}
		set
		{
			SetValue(LabelContentBackgroundProperty, value);
		}
	}

	public Brush LabelTitleBackground
	{
		get
		{
			return (Brush)GetValue(LabelTitleBackgroundProperty);
		}
		set
		{
			SetValue(LabelTitleBackgroundProperty, value);
		}
	}

	public Thickness LabelTitlePanelPadding
	{
		get
		{
			return (Thickness)GetValue(LabelTitlePanelPaddingProperty);
		}
		set
		{
			SetValue(LabelTitlePanelPaddingProperty, value);
		}
	}

	public Thickness LabelContentPanelPadding
	{
		get
		{
			return (Thickness)GetValue(LabelContentPanelPaddingProperty);
		}
		set
		{
			SetValue(LabelContentPanelPaddingProperty, value);
		}
	}

	public string Title
	{
		get
		{
			return (string)GetValue(TitleProperty);
		}
		set
		{
			SetValue(TitleProperty, value);
		}
	}

	public double LableTitlePanelWidth
	{
		get
		{
			return (double)GetValue(LableTitlePanelWidthProperty);
		}
		set
		{
			SetValue(LableTitlePanelWidthProperty, value);
		}
	}

	public double BarMinValue
	{
		get
		{
			return (double)GetValue(BarMinValueProperty);
		}
		set
		{
			SetValue(BarMinValueProperty, value);
		}
	}

	public double BarMaxValue
	{
		get
		{
			return (double)GetValue(BarMaxValueProperty);
		}
		set
		{
			SetValue(BarMaxValueProperty, value);
		}
	}

	public double BarValue
	{
		get
		{
			return (double)GetValue(BarValueProperty);
		}
		set
		{
			SetValue(BarValueProperty, value);
		}
	}
}
