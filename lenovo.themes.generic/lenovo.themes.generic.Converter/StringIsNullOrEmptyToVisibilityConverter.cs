using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class StringIsNullOrEmptyToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool flag = true;
		if (parameter != null)
		{
			flag = (bool)parameter;
		}
		bool flag2 = value == null || value.ToString().Length == 0;
		if (flag)
		{
			return flag2 ? Visibility.Collapsed : Visibility.Visible;
		}
		return (!flag2) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
