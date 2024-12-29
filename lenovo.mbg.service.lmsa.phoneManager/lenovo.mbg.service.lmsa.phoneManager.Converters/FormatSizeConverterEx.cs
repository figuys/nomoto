using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class FormatSizeConverterEx : IValueConverter
{
	public static string FormatSize(string data)
	{
		Match match = Regex.Match(data, "^(?<value>[\\d\\.]+)(?<unit>.*)$");
		string value = match.Groups["unit"].Value;
		double result = 0.0;
		double.TryParse(match.Groups["value"].Value, out result);
		return result.ToString("###,###,###,##0.##") + value;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return string.Empty;
		}
		return FormatSize(value.ToString());
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value;
	}
}
