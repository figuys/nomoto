using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class ProgressPercentMarginConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		double result = 0.0;
		double result2 = 0.0;
		double num = 0.0;
		if (values != null && values.Length == 2)
		{
			double.TryParse(values[0].ToString(), out result);
			double.TryParse(values[1].ToString(), out result2);
		}
		num = ((result2 != 1.0) ? (result * result2 - 13.0) : (result - 22.0));
		if (num >= result - 22.0)
		{
			num = result - 22.0;
		}
		return new Thickness(num, 0.0, 0.0, 0.0);
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
