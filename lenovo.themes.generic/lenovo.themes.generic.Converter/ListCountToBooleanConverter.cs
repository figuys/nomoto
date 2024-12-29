using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class ListCountToBooleanConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		PropertyInfo property = value.GetType().GetProperty("Count");
		if (property == null)
		{
			return false;
		}
		object value2 = property.GetValue(value);
		if (value2 == null)
		{
			return false;
		}
		return !value2.Equals(0);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
