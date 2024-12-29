using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace lenovo.themes.generic.Converter;

public class CompareValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (parameter == null)
		{
			return null;
		}
		string[] array = (parameter as string).Split('#')[0].Split(':');
		string[] array2 = array[0].ToLower().Split('|');
		if (value == null && array2[0] == "null")
		{
			return array[1];
		}
		if (value == null)
		{
			return array[2];
		}
		string value2 = value.ToString().ToLower();
		if (!array2.Contains(value2))
		{
			return array[2];
		}
		return array[1];
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = parameter as string;
		if (text.Contains('|'))
		{
			return Binding.DoNothing;
		}
		if (text.Contains('#'))
		{
			string[] array = text.Split('#', ':');
			if (value == null)
			{
				return array[3];
			}
			if (!array[1].Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return array[3];
			}
			return array[0];
		}
		string[] array2 = text.Split(':');
		if (value == null)
		{
			return Binding.DoNothing;
		}
		if (array2[1].Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
		{
			return array2[0];
		}
		return Binding.DoNothing;
	}
}
