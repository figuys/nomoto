using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.Controls;

public class StatusButton : Button
{
	public static readonly DependencyProperty ButtonSatusProperty = DependencyProperty.Register("ButtonSatus", typeof(ButtonSatus), typeof(StatusButton), new PropertyMetadata(ButtonSatus.NORNAM));

	public static readonly DependencyProperty StatucButtonIconVisibilityProperty = DependencyProperty.Register("StatucButtonIconVisibility", typeof(Visibility), typeof(StatusButton), new PropertyMetadata(Visibility.Collapsed));

	public static readonly DependencyProperty StatusButtonIconProperty = DependencyProperty.Register("StatusButtonIcon", typeof(object), typeof(IconButton), new PropertyMetadata(null));

	public ButtonSatus ButtonSatus
	{
		get
		{
			return (ButtonSatus)GetValue(ButtonSatusProperty);
		}
		set
		{
			SetValue(ButtonSatusProperty, value);
		}
	}

	public Visibility StatucButtonIconVisibility
	{
		get
		{
			return (Visibility)GetValue(StatucButtonIconVisibilityProperty);
		}
		set
		{
			SetValue(StatucButtonIconVisibilityProperty, value);
		}
	}

	public object StatusButtonIcon
	{
		get
		{
			return GetValue(StatusButtonIconProperty);
		}
		set
		{
			SetValue(StatusButtonIconProperty, value);
		}
	}
}
