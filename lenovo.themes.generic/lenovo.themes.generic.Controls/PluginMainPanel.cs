using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.Controls;

public class PluginMainPanel : Control
{
	public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(PluginMainPanel), new PropertyMetadata(null));

	public static readonly DependencyProperty StateBarProperty = DependencyProperty.Register("StateBar", typeof(object), typeof(PluginMainPanel), new PropertyMetadata(null));

	public object Content
	{
		get
		{
			return GetValue(ContentProperty);
		}
		set
		{
			SetValue(ContentProperty, value);
		}
	}

	public object StateBar
	{
		get
		{
			return GetValue(StateBarProperty);
		}
		set
		{
			SetValue(StateBarProperty, value);
		}
	}
}
