using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class NotEqualsToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null && parameter == null)
		{
			return Visibility.Collapsed;
		}
		if (value != null && parameter == null)
		{
			return Visibility.Visible;
		}
		if (value == null && parameter != null)
		{
			return Visibility.Visible;
		}
		return value.Equals(parameter) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
