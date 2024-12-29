using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Comm;

public class Number2TimeStrConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return "00:00:00";
		}
		int num = (int)(double)value;
		return $"{num / 3600:00}:{num % 3600 / 60:00}:{num % 60:00}";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
