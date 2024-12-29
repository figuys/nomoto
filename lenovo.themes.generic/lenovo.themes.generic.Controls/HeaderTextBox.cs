using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.Controls;

public class HeaderTextBox : TextBox
{
	public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HeaderTextBox), new PropertyMetadata(null));

	public static readonly DependencyProperty EmptyValTipProperty = DependencyProperty.Register("EmptyValTip", typeof(object), typeof(HeaderTextBox), new PropertyMetadata(null));

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

	public object TipWhenTextIsEmpty
	{
		get
		{
			return GetValue(EmptyValTipProperty);
		}
		set
		{
			SetValue(EmptyValTipProperty, value);
		}
	}
}
