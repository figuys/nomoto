using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class SingleObjectConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string[] array = parameter.ToString().Split(':');
		if (value == null)
		{
			return array[2];
		}
		if (array[0].Contains("|"))
		{
			if (!array[0].ToLower().Split('|').Contains(value.ToString().ToLower()))
			{
				return array[2];
			}
			return array[1];
		}
		if (!array[0].ToLower().Equals(value.ToString().ToLower()))
		{
			return array[2];
		}
		return array[1];
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string result = "otherValue";
		string[] array = parameter.ToString().ToLower().Split(':');
		if (value == null)
		{
			return result;
		}
		if (value.ToString().ToLower() != array[1])
		{
			return result;
		}
		if (!array[0].Contains('|'))
		{
			return array[0];
		}
		return array[0].Split('|')[0];
	}
}
