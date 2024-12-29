using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(DateTime), typeof(string))]
public class CallDateConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return ((DateTime)value).ToString("yyyy-MM-dd HH:mm");
	}
}
