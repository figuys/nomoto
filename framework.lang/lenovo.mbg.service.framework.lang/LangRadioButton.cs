using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.framework.lang;

public class LangRadioButton : RadioButton
{
	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(LangRadioButton), new PropertyMetadata(string.Empty, OnLangKeyChanged));

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
			((LangRadioButton)obj).Content = LangTranslation.Translate(args.NewValue.ToString());
		}
	}
}
