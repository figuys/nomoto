using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.framework.lang;

public class LangButton : Button
{
	[Obsolete("LangContent 不再使用，替换为 LangKey")]
	public static readonly DependencyProperty LangContentProperty = DependencyProperty.Register("LangContent", typeof(object), typeof(LangButton), new PropertyMetadata(null, null));

	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(LangButton), new PropertyMetadata(string.Empty, OnLangKeyChanged));

	public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(LangButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(LangButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register("DisabledBackground", typeof(Brush), typeof(LangButton), new PropertyMetadata(null));

	public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register("DisabledForeground", typeof(Brush), typeof(LangButton), new PropertyMetadata(null));

	public static readonly DependencyProperty BorderCornerRadiusProperty = DependencyProperty.Register("BorderCornerRadius", typeof(CornerRadius), typeof(LangButton), new UIPropertyMetadata(default(CornerRadius)));

	[Obsolete("LangContent 不再使用，替换为 LangKey")]
	public object LangContent
	{
		get
		{
			return GetValue(LangContentProperty);
		}
		set
		{
			SetValue(LangContentProperty, value);
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

	public CornerRadius BorderCornerRadius
	{
		get
		{
			return (CornerRadius)GetValue(BorderCornerRadiusProperty);
		}
		set
		{
			SetValue(BorderCornerRadiusProperty, value);
		}
	}

	private static void OnLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue != null && args.NewValue != args.OldValue)
		{
			((LangButton)obj).Content = LangTranslation.Translate(args.NewValue.ToString());
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
