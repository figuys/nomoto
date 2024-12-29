using System;
using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.AttachedProperty;

public class WebBrowser
{
	public static readonly DependencyProperty StringSourceProperty = DependencyProperty.RegisterAttached("StringSource", typeof(string), typeof(WebBrowser), new PropertyMetadata(string.Empty, StringSourcePropertyChangedCallback));

	public static string GetStringSource(DependencyObject obj)
	{
		return (string)obj.GetValue(StringSourceProperty);
	}

	public static void SetStringSource(DependencyObject obj, string value)
	{
		obj.SetValue(StringSourceProperty, value);
	}

	private static void StringSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue == null)
		{
			return;
		}
		string text = e.NewValue.ToString();
		System.Windows.Controls.WebBrowser webBrowser = d as System.Windows.Controls.WebBrowser;
		try
		{
			webBrowser.NavigateToString(text);
		}
		catch (Exception)
		{
		}
	}
}
