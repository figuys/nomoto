using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class InvertBoolConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = parameter as string;
		if (string.IsNullOrEmpty(text) || text.Split(':').Length != 3)
		{
			if (value == null || !bool.TryParse(value.ToString(), out var result))
			{
				return false;
			}
			return !result;
		}
		string[] array = text.Split(':');
		if (value == null)
		{
			return array[2];
		}
		if (!(value.ToString() == array[0]))
		{
			return array[2];
		}
		return array[1];
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = parameter as string;
		if (string.IsNullOrEmpty(text) || text.Split(':').Length != 3)
		{
			if (value == null || !bool.TryParse(value.ToString(), out var result))
			{
				return true;
			}
			return !result;
		}
		string[] array = text.Split(':');
		if (value == null)
		{
			return false;
		}
		return (value.ToString() == array[1]) ? true : false;
	}
}
