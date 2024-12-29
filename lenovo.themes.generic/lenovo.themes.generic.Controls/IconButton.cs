using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.framework.lang;

namespace lenovo.themes.generic.Controls;

public class IconButton : Button
{
	public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(IconButton), new PropertyMetadata(new Thickness(0.0)));

	public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(GridLength), typeof(IconButton), new PropertyMetadata(GridLength.Auto));

	public static readonly DependencyProperty IconHorizontalAlignmentProperty = DependencyProperty.Register("IconHorizontalAlignment", typeof(HorizontalAlignment), typeof(IconButton), new PropertyMetadata(HorizontalAlignment.Center));

	public static readonly DependencyProperty IconVerticalAlignmentProperty = DependencyProperty.Register("IconVerticalAlignment", typeof(VerticalAlignment), typeof(IconButton), new PropertyMetadata(VerticalAlignment.Center));

	public static readonly DependencyProperty MouseOverIconProperty = DependencyProperty.Register("MouseOverIcon", typeof(object), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledIconProperty = DependencyProperty.Register("DisabledIcon", typeof(object), typeof(IconButton), new PropertyMetadata(0));

	public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty HeaderMarginProperty = DependencyProperty.Register("HeaderMargin", typeof(Thickness), typeof(IconButton), new PropertyMetadata(new Thickness(0.0)));

	public static readonly DependencyProperty HeaderHorizontalAlignmentProperty = DependencyProperty.Register("HeaderHorizontalAlignment", typeof(HorizontalAlignment), typeof(IconButton), new PropertyMetadata(HorizontalAlignment.Center));

	public static readonly DependencyProperty HeaderVerticalAlignmentProperty = DependencyProperty.Register("HeaderVerticalAlignment", typeof(VerticalAlignment), typeof(IconButton), new PropertyMetadata(VerticalAlignment.Center));

	public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.Register("HeaderWidth", typeof(GridLength), typeof(IconButton), new PropertyMetadata(GridLength.Auto));

	public static readonly DependencyProperty MouseOverHeaderProperty = DependencyProperty.Register("MouseOverHeader", typeof(object), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledHeaderProperty = DependencyProperty.Register("DisabledHeader", typeof(object), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

	public static readonly DependencyProperty NormalBackgroundProperty = DependencyProperty.Register("NormalBackground", typeof(Brush), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty NormalForegroundProperty = DependencyProperty.Register("NormalForeground", typeof(Brush), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register("DisabledBackground", typeof(Brush), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register("DisabledForeground", typeof(Brush), typeof(IconButton), new PropertyMetadata(null));

	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(IconButton), new PropertyMetadata(string.Empty, OnLangKeyChanged));

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

	public object Icon
	{
		get
		{
			return GetValue(IconProperty);
		}
		set
		{
			SetValue(IconProperty, value);
		}
	}

	public Thickness IconMargin
	{
		get
		{
			return (Thickness)GetValue(IconMarginProperty);
		}
		set
		{
			SetValue(IconMarginProperty, value);
		}
	}

	public GridLength IconWidth
	{
		get
		{
			return (GridLength)GetValue(IconWidthProperty);
		}
		set
		{
			SetValue(IconWidthProperty, value);
		}
	}

	public HorizontalAlignment IconHorizontalAlignment
	{
		get
		{
			return (HorizontalAlignment)GetValue(IconHorizontalAlignmentProperty);
		}
		set
		{
			SetValue(IconHorizontalAlignmentProperty, value);
		}
	}

	public VerticalAlignment IconVerticalAlignment
	{
		get
		{
			return (VerticalAlignment)GetValue(IconVerticalAlignmentProperty);
		}
		set
		{
			SetValue(IconVerticalAlignmentProperty, value);
		}
	}

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

	public object Header
	{
		get
		{
			return GetValue(HeaderProperty);
		}
		set
		{
			SetValue(HeaderProperty, value);
		}
	}

	public Thickness HeaderMargin
	{
		get
		{
			return (Thickness)GetValue(HeaderMarginProperty);
		}
		set
		{
			SetValue(HeaderMarginProperty, value);
		}
	}

	public HorizontalAlignment HeaderHorizontalAlignment
	{
		get
		{
			return (HorizontalAlignment)GetValue(HeaderHorizontalAlignmentProperty);
		}
		set
		{
			SetValue(HeaderHorizontalAlignmentProperty, value);
		}
	}

	public VerticalAlignment HeaderVerticalAlignment
	{
		get
		{
			return (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty);
		}
		set
		{
			SetValue(HeaderVerticalAlignmentProperty, value);
		}
	}

	public GridLength HeaderWidth
	{
		get
		{
			return (GridLength)GetValue(HeaderWidthProperty);
		}
		set
		{
			SetValue(HeaderWidthProperty, value);
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

	public Brush NormalBackground
	{
		get
		{
			return (Brush)GetValue(NormalBackgroundProperty);
		}
		set
		{
			SetValue(NormalBackgroundProperty, value);
		}
	}

	public Brush NormalForeground
	{
		get
		{
			return (Brush)GetValue(NormalForegroundProperty);
		}
		set
		{
			SetValue(NormalForegroundProperty, value);
		}
	}

	public Brush DisabledBackground
	{
		get
		{
			return (Brush)GetValue(DisabledBackgroundProperty);
		}
		set
		{
			SetValue(DisabledBackgroundProperty, value);
		}
	}

	public Brush DisabledForeground
	{
		get
		{
			return (Brush)GetValue(DisabledForegroundProperty);
		}
		set
		{
			SetValue(DisabledForegroundProperty, value);
		}
	}

	public string LangKey
	{
		get
		{
			return (string)GetValue(LangKeyProperty);
		}
		set
		{
			SetValue(LangKeyProperty, value);
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

	private static void OnLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue != null && args.NewValue != args.OldValue)
		{
			((IconButton)obj).Content = LangTranslation.Translate(args.NewValue.ToString());
		}
	}
}
