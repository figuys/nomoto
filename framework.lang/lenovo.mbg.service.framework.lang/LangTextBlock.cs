using System;
using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.framework.lang;

public class LangTextBlock : TextBlock
{
	[Obsolete("LangText 不再使用，替换为 LangKey")]
	public static readonly DependencyProperty LangTextProperty = DependencyProperty.Register("LangText", typeof(string), typeof(LangTextBlock), new PropertyMetadata(string.Empty, null));

	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(LangTextBlock), new PropertyMetadata(string.Empty, OnLangKeyChanged));

	[Obsolete("LangText 不再使用，替换为 LangKey")]
	public string LangText
	{
		get
		{
			return (string)GetValue(LangTextProperty);
		}
		set
		{
			SetValue(LangTextProperty, value);
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
		TextBlock textBlock = obj as TextBlock;
		textBlock.Inlines.Clear();
		if (args.NewValue != null && args.NewValue != args.OldValue)
		{
			textBlock.Text = LangTranslation.Translate(args.NewValue.ToString());
		}
	}
}
