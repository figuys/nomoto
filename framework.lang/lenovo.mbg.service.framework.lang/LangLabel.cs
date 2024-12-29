using System;
using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.framework.lang;

public class LangLabel : Label
{
	[Obsolete("LangContent 只记录英文文字，请使用 LangKey")]
	public static readonly DependencyProperty LangContentProperty = DependencyProperty.Register("LangContent", typeof(object), typeof(LangLabel), new PropertyMetadata(null, null));

	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(LangLabel), new PropertyMetadata(string.Empty, OnLangKeyChanged));

	[Obsolete("LangContent 只记录英文文字，请使用 LangKey")]
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

	private static void OnLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue != null && args.NewValue != args.OldValue)
		{
			((LangLabel)obj).Content = LangTranslation.Translate(args.NewValue.ToString());
		}
	}
}
