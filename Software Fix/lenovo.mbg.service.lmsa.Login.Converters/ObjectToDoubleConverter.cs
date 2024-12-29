using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.Login.Converters;

public class ObjectToDoubleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 0;
		}
		double result = 0.0;
		double.TryParse(value.ToString(), out result);
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
