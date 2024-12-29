using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.Converters;

public class Bool2VisiableConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (parameter != null && parameter.ToString() == "INVERT")
		{
			if (value == null || !System.Convert.ToBoolean(value))
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
		if (value == null || !System.Convert.ToBoolean(value))
		{
			return Visibility.Collapsed;
		}
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
