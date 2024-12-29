using System.Reflection;
using System.Windows;

namespace lenovo.mbg.service.framework.lang;

public class Lang
{
	public static readonly DependencyProperty LangSourceProperty = DependencyProperty.RegisterAttached("LangSource", typeof(LangSource), typeof(Lang), new PropertyMetadata(null, delegate(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		string path = ((LangSource)args.NewValue).Path;
		if (!string.IsNullOrEmpty(path))
		{
			PropertyInfo property = obj.GetType().GetProperty(path);
			if (!(property == null))
			{
				string value = LangTranslation.Translate(property.GetValue(obj)?.ToString());
				property.SetValue(obj, value);
			}
		}
	}));

	public static LangSource GetLangSource(DependencyObject obj)
	{
		return (LangSource)obj.GetValue(LangSourceProperty);
	}

	public static void SetLangSource(DependencyObject obj, LangSource value)
	{
		obj.SetValue(LangSourceProperty, value);
	}
}
