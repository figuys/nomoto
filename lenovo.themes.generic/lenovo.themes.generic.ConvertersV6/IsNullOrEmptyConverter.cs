using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class IsNullOrEmptyConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return true;
		}
		if (string.IsNullOrEmpty(value.ToString().Trim()))
		{
			return true;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
