using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(DateTime), typeof(string))]
public class CallDurationConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		long num = (long)value;
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num).ToString("HH:mm:ss");
	}
}
