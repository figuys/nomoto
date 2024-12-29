using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace lenovo.themes.generic.Controls;

public class SearchTextBox : TextBox
{
	public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchTextBox), new PropertyMetadata(null));

	public static readonly DependencyProperty StopSearchCommandProperty = DependencyProperty.Register("StopSearchCommand", typeof(ICommand), typeof(SearchTextBox), new PropertyMetadata(null));

	public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(SearchTextBox), new PropertyMetadata(false));

	public ICommand SearchCommand
	{
		get
		{
			return (ICommand)GetValue(SearchCommandProperty);
		}
		set
		{
			SetValue(SearchCommandProperty, value);
		}
	}

	public ICommand StopSearchCommand
	{
		get
		{
			return (ICommand)GetValue(StopSearchCommandProperty);
		}
		set
		{
			SetValue(StopSearchCommandProperty, value);
		}
	}

	public bool IsSearching
	{
		get
		{
			return (bool)GetValue(IsSearchingProperty);
		}
		set
		{
			SetValue(IsSearchingProperty, value);
		}
	}
}
