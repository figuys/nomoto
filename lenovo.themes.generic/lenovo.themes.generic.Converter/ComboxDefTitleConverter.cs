using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class ComboxDefTitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length < 2)
		{
			return null;
		}
		if (System.Convert.ToInt32(values[0]) == -1)
		{
			return values[2] as string;
		}
		return values[1];
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
