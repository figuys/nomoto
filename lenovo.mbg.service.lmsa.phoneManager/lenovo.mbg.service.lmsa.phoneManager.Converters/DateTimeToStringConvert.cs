using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class DateTimeToStringConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		int hour = DateTime.Now.Hour;
		if (hour > 0 && hour < 12)
		{
			return "Good morning!";
		}
		if (hour < 18)
		{
			return "Good afternoon!";
		}
		return "Good evening!";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
