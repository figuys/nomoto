using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa;

public class ImageButton : Button
{
	public static readonly DependencyProperty ButtonImageSourceProperty = DependencyProperty.Register("ButtonImageSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(ImageButton), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty NormalBackgroundBrushProperty = DependencyProperty.Register("NormalBackgroundBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MousePressDownBackgroundBrushProperty = DependencyProperty.Register("MousePressDownBackgroundBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundBrushProperty = DependencyProperty.Register("MouseOverBackgroundBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledBackgroundBrushProperty = DependencyProperty.Register("DisabledBackgroundBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));

	public ImageSource ButtonImageSource
	{
		get
		{
			return (ImageSource)GetValue(ButtonImageSourceProperty);
		}
		set
		{
			SetValue(ButtonImageSourceProperty, value);
		}
	}

	public string ButtonText
	{
		get
		{
			return (string)GetValue(ButtonTextProperty);
		}
		set
		{
			SetValue(ButtonTextProperty, value);
		}
	}

	public Brush NormalBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(NormalBackgroundBrushProperty);
		}
		set
		{
			SetValue(NormalBackgroundBrushProperty, value);
		}
	}

	public Brush MousePressDownBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(MousePressDownBackgroundBrushProperty);
		}
		set
		{
			SetValue(MousePressDownBackgroundBrushProperty, value);
		}
	}

	public Brush MouseOverBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(MouseOverBackgroundBrushProperty);
		}
		set
		{
			SetValue(MouseOverBackgroundBrushProperty, value);
		}
	}

	public Brush DisabledBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(DisabledBackgroundBrushProperty);
		}
		set
		{
			SetValue(DisabledBackgroundBrushProperty, value);
		}
	}
}
