using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class IntToStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		int result = 0;
		int.TryParse(value.ToString(), out result);
		if (parameter != null)
		{
			return string.Format(parameter.ToString(), result.ToString());
		}
		return result.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
