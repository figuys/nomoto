using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.ControlsV6;

public class PasswordBoxBehaviorV6
{
	public static readonly DependencyProperty IsValideProperty = DependencyProperty.RegisterAttached("IsValide", typeof(bool), typeof(PasswordBoxBehaviorV6), new PropertyMetadata(true));

	public static readonly DependencyProperty PasswordLengthProperty = DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxBehaviorV6), new PropertyMetadata(0));

	public static readonly DependencyProperty IsPasswordMonitorProperty = DependencyProperty.RegisterAttached("IsPasswordMonitor", typeof(bool), typeof(PasswordBoxBehaviorV6), new UIPropertyMetadata(false, OnIsMonitoringChanged));

	public static readonly DependencyProperty IsPasswordBindingEnabledProperty = DependencyProperty.RegisterAttached("IsPasswordBindingEnabled", typeof(bool), typeof(PasswordBoxBehaviorV6), new UIPropertyMetadata(false, OnIsPasswordBindingEnabledChanged));

	public static readonly DependencyProperty BindedPasswordProperty = DependencyProperty.RegisterAttached("BindedPassword", typeof(string), typeof(PasswordBoxBehaviorV6), new UIPropertyMetadata(string.Empty, OnBindedPasswordChanged));

	public static bool GetIsValide(DependencyObject obj)
	{
		return (bool)obj.GetValue(IsValideProperty);
	}

	public static void SetIsValide(DependencyObject obj, bool value)
	{
		obj.SetValue(IsValideProperty, value);
	}

	public static int GetPasswordLength(DependencyObject obj)
	{
		return (int)obj.GetValue(PasswordLengthProperty);
	}

	public static void SetPasswordLength(DependencyObject obj, int value)
	{
		obj.SetValue(PasswordLengthProperty, value);
	}

	public static bool GetIsPasswordMonitor(DependencyObject obj)
	{
		return (bool)obj.GetValue(IsPasswordMonitorProperty);
	}

	public static void SetIsPasswordMonitor(DependencyObject obj, bool value)
	{
		obj.SetValue(IsPasswordMonitorProperty, value);
	}

	private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is PasswordBox passwordBox)
		{
			if ((bool)e.NewValue)
			{
				passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
			}
			else
			{
				passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
			}
		}
	}

	public static bool GetIsPasswordBindingEnabled(DependencyObject obj)
	{
		return (bool)obj.GetValue(IsPasswordBindingEnabledProperty);
	}

	public static void SetIsPasswordBindingEnabled(DependencyObject obj, bool value)
	{
		obj.SetValue(IsPasswordBindingEnabledProperty, value);
	}

	private static void OnIsPasswordBindingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (obj is PasswordBox passwordBox)
		{
			passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
			if ((bool)e.NewValue)
			{
				passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
			}
		}
	}

	private static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
	{
		PasswordBox passwordBox = (PasswordBox)sender;
		if (passwordBox != null)
		{
			if (!string.Equals(GetBindedPassword(passwordBox), passwordBox.Password))
			{
				SetBindedPassword(passwordBox, passwordBox.Password);
			}
			SetPasswordLength(passwordBox, passwordBox.Password.Length);
			SetPasswordBoxSelection(passwordBox, passwordBox.Password.Length + 1, passwordBox.Password.Length + 1);
		}
	}

	public static string GetBindedPassword(DependencyObject obj)
	{
		return (string)obj.GetValue(BindedPasswordProperty);
	}

	public static void SetBindedPassword(DependencyObject obj, string value)
	{
		obj.SetValue(BindedPasswordProperty, value);
	}

	private static void OnBindedPasswordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (obj is PasswordBox passwordBox)
		{
			passwordBox.Password = ((e.NewValue == null) ? string.Empty : e.NewValue.ToString());
		}
	}

	private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
	{
		passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[2] { start, length });
	}
}
