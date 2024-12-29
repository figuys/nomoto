using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

[ValueConversion(typeof(long), typeof(string))]
public class FormatSizeConverter : IValueConverter
{
	public static string FormatSize(long size)
	{
		if (size == 0L)
		{
			return "0 KB";
		}
		float num = (float)size / 1024f;
		if ((double)num <= 999.99)
		{
			return num.ToString("#0.00") + " KB";
		}
		float num2 = num / 1024f;
		if ((double)num2 <= 999.99)
		{
			return num2.ToString("###,###,###,##0.##") + " MB";
		}
		return (num2 / 1024f).ToString("###,###,###,##0.##") + " GB";
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			long num = System.Convert.ToInt64(value);
			if (num < 0)
			{
				return "";
			}
			return FormatSize(num);
		}
		catch
		{
			return "";
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 0;
		}
		return FormatSize(value.ToString());
	}

	private long FormatSize(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0L;
		}
		Match match = Regex.Match(str.Trim(), "^(?<value>[\\d.]+)\\s*(?<unit>[A-Za-z]).*$");
		string value = match.Groups["value"].Value;
		string text = match.Groups["unit"].Value.ToLower();
		double result = 0.0;
		double.TryParse(value, out result);
		return text switch
		{
			"k" => (long)(result * 1024.0), 
			"m" => (long)(result * 1024.0 * 1024.0), 
			"g" => (long)(result * 1024.0 * 1024.0 * 1024.0), 
			"t" => (long)(result * 1024.0 * 1024.0 * 1024.0 * 1024.0), 
			_ => (long)result, 
		};
	}
}
