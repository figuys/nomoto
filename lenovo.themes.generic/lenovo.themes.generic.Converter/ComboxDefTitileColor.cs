using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class ComboxDefTitileColor : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || (int)value == -1)
		{
			return "#FF9FAEBF";
		}
		return "#FF001429";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
