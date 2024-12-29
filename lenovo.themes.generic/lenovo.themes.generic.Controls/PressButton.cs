using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

public class PressButton : Button
{
	public static readonly DependencyProperty PressUpCommandProperty = DependencyProperty.Register("PressUpCommand", typeof(ReplayCommand), typeof(PressButton), new PropertyMetadata(null));

	public static readonly DependencyProperty PressUpCommandParameterProperty = DependencyProperty.Register("PressUpCommandParameter", typeof(object), typeof(PressButton), new PropertyMetadata(null));

	public ReplayCommand PressUpCommand
	{
		get
		{
			return (ReplayCommand)GetValue(PressUpCommandProperty);
		}
		set
		{
			SetValue(PressUpCommandProperty, value);
		}
	}

	public object PressUpCommandParameter
	{
		get
		{
			return GetValue(PressUpCommandParameterProperty);
		}
		set
		{
			SetValue(PressUpCommandParameterProperty, value);
		}
	}

	protected override void OnMouseDown(MouseButtonEventArgs e)
	{
		base.OnMouseDown(e);
		PressUpCommand?.Execute(PressUpCommandParameter);
	}
}
