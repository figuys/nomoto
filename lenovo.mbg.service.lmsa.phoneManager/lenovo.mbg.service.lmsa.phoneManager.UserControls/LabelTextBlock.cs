using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class LabelTextBlock : Label
{
	public static readonly DependencyProperty LabelContentBackgroundProperty = DependencyProperty.Register("LabelContentBackground", typeof(Brush), typeof(LabelTextBlock), new PropertyMetadata(null));

	public static readonly DependencyProperty LabelTitleBackgroundProperty = DependencyProperty.Register("LabelTitleBackground", typeof(Brush), typeof(LabelTextBlock), new PropertyMetadata(null));

	public static readonly DependencyProperty LabelTitlePanelPaddingProperty = DependencyProperty.Register("LabelTitlePanelPadding", typeof(Thickness), typeof(LabelTextBlock), new PropertyMetadata(default(Thickness)));

	public static readonly DependencyProperty LabelContentPanelPaddingProperty = DependencyProperty.Register("LabelContentPanelPadding", typeof(Thickness), typeof(LabelTextBlock), new PropertyMetadata(default(Thickness)));

	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LabelTextBlock), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty LableTitlePanelWidthProperty = DependencyProperty.Register("LableTitlePanelWidth", typeof(double), typeof(LabelTextBlock), new PropertyMetadata(0.0));

	public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LabelTextBlock), new PropertyMetadata(string.Empty));

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

	public string Text
	{
		get
		{
			return (string)GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}
}
