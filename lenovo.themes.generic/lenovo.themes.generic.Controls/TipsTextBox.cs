using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.Controls;

public class TipsTextBox : TextBox
{
	public static readonly DependencyProperty EmptyTipsProperty = DependencyProperty.Register("EmptyTips", typeof(object), typeof(TipsTextBox), new PropertyMetadata(null));

	public object EmptyTips
	{
		get
		{
			return GetValue(EmptyTipsProperty);
		}
		set
		{
			SetValue(EmptyTipsProperty, value);
		}
	}
}
