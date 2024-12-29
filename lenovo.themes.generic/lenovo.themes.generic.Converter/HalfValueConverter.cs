using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class HalfValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 0;
		}
		double result = 0.0;
		if (double.TryParse(value.ToString(), out result))
		{
			return result / 2.0;
		}
		return 0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
