using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.themes.generic.Controls;

public class MouseOverMenuItem : MenuItem
{
	public static readonly DependencyProperty MouseOverIconProperty = DependencyProperty.Register("MouseOverIcon", typeof(object), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public static readonly DependencyProperty TipsContentProperty = DependencyProperty.Register("TipsContent", typeof(object), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public static readonly DependencyProperty TipsVisibilityProperty = DependencyProperty.Register("TipsVisibility", typeof(Visibility), typeof(MouseOverMenuItem), new PropertyMetadata(Visibility.Collapsed));

	public static readonly DependencyProperty MouseOverHeaderProperty = DependencyProperty.Register("MouseOverHeader", typeof(object), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledIconProperty = DependencyProperty.Register("DisabledIcon", typeof(object), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledHeaderProperty = DependencyProperty.Register("DisabledHeader", typeof(object), typeof(MouseOverMenuItem), new PropertyMetadata(null));

	public object MouseOverIcon
	{
		get
		{
			return GetValue(MouseOverIconProperty);
		}
		set
		{
			SetValue(MouseOverIconProperty, value);
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

	public object TipsContent
	{
		get
		{
			return GetValue(TipsContentProperty);
		}
		set
		{
			SetValue(TipsContentProperty, value);
		}
	}

	public Visibility TipsVisibility
	{
		get
		{
			return (Visibility)GetValue(TipsVisibilityProperty);
		}
		set
		{
			SetValue(TipsVisibilityProperty, value);
		}
	}

	public object MouseOverHeader
	{
		get
		{
			return GetValue(MouseOverHeaderProperty);
		}
		set
		{
			SetValue(MouseOverHeaderProperty, value);
		}
	}

	public object DisabledIcon
	{
		get
		{
			return GetValue(DisabledIconProperty);
		}
		set
		{
			SetValue(DisabledIconProperty, value);
		}
	}

	public object DisabledHeader
	{
		get
		{
			return GetValue(DisabledHeaderProperty);
		}
		set
		{
			SetValue(DisabledHeaderProperty, value);
		}
	}

	public static Brush GetMouseOverBackground(UIElement target)
	{
		return (Brush)target.GetValue(MouseOverBackgroundProperty);
	}

	public static void SetMouseOverBackground(UIElement target, Brush value)
	{
		target.SetValue(MouseOverBackgroundProperty, value);
	}

	public static Brush GetMouseOverForeground(UIElement target)
	{
		return (Brush)target.GetValue(MouseOverForegroundProperty);
	}

	public static void SetMouseOverForeground(UIElement target, Brush value)
	{
		target.SetValue(MouseOverForegroundProperty, value);
	}
}
