using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class WarrantyVisibilityConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		double num = (double)values[0];
		double num2 = (double)values[1] - num;
		if ((bool)values[2])
		{
			return (num2 < 3.0) ? Visibility.Collapsed : Visibility.Visible;
		}
		return (num2 < 6.0) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
