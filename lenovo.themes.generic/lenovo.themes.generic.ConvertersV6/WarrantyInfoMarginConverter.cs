using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class WarrantyInfoMarginConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values.Length == 1)
		{
			if ((double)values[0] >= 19.0)
			{
				return new Thickness(-50.0, 0.0, 0.0, 0.0);
			}
		}
		else if (values.Length == 3)
		{
			double num = (double)values[0];
			double num2 = (double)values[1];
			double num3 = (double)values[2];
			double num4 = (num2 - num) / 24.0 * num3;
			num4 += 7.0;
			if (num4 >= num3)
			{
				num4 = num3 - 10.0;
			}
			return new Thickness(num4, 0.0, 0.0, 0.0);
		}
		return new Thickness(0.0, 0.0, 0.0, 0.0);
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
