using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class WarrantyInfoProgressWidthConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		double num = (double)values[0];
		double num2 = (double)values[1];
		double num3 = (double)values[2];
		double num4 = (num2 - num) / 24.0 * num3;
		if (num4 > 0.0)
		{
			num4 += 5.0;
		}
		if (num4 > num3)
		{
			num4 = num3;
		}
		return num4;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
