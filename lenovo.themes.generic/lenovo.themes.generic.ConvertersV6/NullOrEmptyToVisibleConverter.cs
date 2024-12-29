using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class NullOrEmptyToVisibleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Visibility visibility = Visibility.Collapsed;
		if (parameter != null && parameter.ToString() == "Hidden")
		{
			visibility = Visibility.Hidden;
		}
		if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
		{
			visibility = Visibility.Visible;
		}
		return visibility;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
