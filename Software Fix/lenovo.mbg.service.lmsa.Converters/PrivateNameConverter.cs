using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.Converters;

public class PrivateNameConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		string text = value as string;
		if (Regex.IsMatch(text, "^[\\w-.]+@[\\w-]+(\\.[\\w-]+)+$"))
		{
			return Regex.Replace(text, "[\\w-.]{1,3}@", "***@");
		}
		if (string.IsNullOrEmpty(text) || text.Length <= 3)
		{
			return text;
		}
		int length = text.Length - 3;
		return text.Substring(0, length) + "***";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
