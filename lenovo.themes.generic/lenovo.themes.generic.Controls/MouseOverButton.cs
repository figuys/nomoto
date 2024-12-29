using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.themes.generic.Controls;

public class MouseOverButton : Button
{
	public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MouseOverButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(MouseOverButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(MouseOverButton), new PropertyMetadata(null));

	public CornerRadius CornerRadius
	{
		get
		{
			return (CornerRadius)GetValue(CornerRadiusProperty);
		}
		set
		{
			SetValue(CornerRadiusProperty, value);
		}
	}

	public Brush MouseOverForeground
	{
		get
		{
			return (Brush)GetValue(MouseOverForegroundProperty);
		}
		set
		{
			SetValue(MouseOverForegroundProperty, value);
		}
	}

	public Brush MouseOverBackground
	{
		get
		{
			return (Brush)GetValue(MouseOverBackgroundProperty);
		}
		set
		{
			SetValue(MouseOverBackgroundProperty, value);
		}
	}

	public static Brush GetMouseOverForeground(UIElement target)
	{
		return (Brush)target.GetValue(MouseOverForegroundProperty);
	}

	public static void SetMouseOverForeground(UIElement target, Brush value)
	{
		target.SetValue(MouseOverForegroundProperty, value);
	}

	public static Brush GetMouseOverBackground(UIElement target)
	{
		return (Brush)target.GetValue(MouseOverBackgroundProperty);
	}

	public static void SetMouseOverBackground(UIElement target, Brush value)
	{
		target.SetValue(MouseOverBackgroundProperty, value);
	}
}
