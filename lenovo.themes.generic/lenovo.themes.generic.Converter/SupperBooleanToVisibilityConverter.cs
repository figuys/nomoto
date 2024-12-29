using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class SupperBooleanToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Visible;
		}
		bool num = parameter == null || (bool)parameter;
		bool flag = (bool)value;
		return (!num) ? (flag ? Visibility.Collapsed : Visibility.Visible) : ((!flag) ? Visibility.Collapsed : Visibility.Visible);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
