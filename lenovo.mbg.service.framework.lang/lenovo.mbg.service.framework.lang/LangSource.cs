using System.Windows;

namespace lenovo.mbg.service.framework.lang;

public class LangSource : DependencyObject
{
	public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(LangSource), new PropertyMetadata(string.Empty));

	public string Path
	{
		get
		{
			return (string)GetValue(PathProperty);
		}
		set
		{
			SetValue(PathProperty, value);
		}
	}
}
