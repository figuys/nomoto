using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.Selector;

public class StrItemSelector : StyleSelector
{
	public Style NormalStyle { get; set; }

	public Style OtherStyle { get; set; }

	public override Style SelectStyle(object item, DependencyObject container)
	{
		string obj = item as string;
		if (obj != null && obj.Contains("1."))
		{
			return OtherStyle;
		}
		return NormalStyle;
	}
}
