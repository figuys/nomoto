using System.Windows;

namespace lenovo.themes.generic.Controls;

public class TextChangedRoutedEventArgs : RoutedEventArgs
{
	public string Text { get; set; }

	public TextChangedRoutedEventArgs(string text, RoutedEvent routedEvent)
		: base(routedEvent)
	{
		Text = text;
	}
}
