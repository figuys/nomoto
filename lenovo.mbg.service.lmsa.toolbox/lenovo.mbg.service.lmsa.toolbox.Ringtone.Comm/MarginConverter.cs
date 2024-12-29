using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Comm;

public class MarginConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return new Thickness(-5.0);
		}
		if (parameter == null)
		{
			return new Thickness((double)value - 10.0, -5.0, -5.0, -5.0);
		}
		return new Thickness(-5.0, -5.0, (double)value - 10.0, -5.0);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
