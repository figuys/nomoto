using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(DateTime), typeof(string))]
public class CalendarConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DateTime dateTime = (DateTime)value;
		string text = parameter as string;
		if (!string.IsNullOrEmpty(text))
		{
			return dateTime.ToString(text);
		}
		return dateTime.ToShortDateString();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (DateTime.TryParse((string)value, out var result))
		{
			return result;
		}
		return DateTime.Now;
	}
}
