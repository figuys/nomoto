using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(long), typeof(string))]
public class TicksToDateConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			if (value == null)
			{
				return string.Empty;
			}
			long num = long.Parse((string)value);
			return new DateTime(1970, 1, 1).AddMilliseconds(num).ToLocalTime().ToString("yyyy-MM-dd HH:mm");
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}
}
