using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class FileCountAndSizeConverter : IMultiValueConverter
{
	public static string FormatSize(object size)
	{
		string result = string.Empty;
		try
		{
			long num = (long)size;
			if ((double)num < 1024.0)
			{
				result = num.ToString("F2") + " Byte";
			}
			else if ((double)num >= 1024.0 && num < 1048576)
			{
				result = ((double)num / 1024.0).ToString("F2") + " K";
			}
			else if (num >= 1048576 && num < 1073741824)
			{
				result = ((double)num / 1024.0 / 1024.0).ToString("F2") + " M";
			}
			else if (num >= 1073741824)
			{
				result = ((double)num / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " G";
			}
		}
		catch
		{
		}
		return result;
	}

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		try
		{
			FormatSize(values[1]);
			result = $"{values[0]} Photos";
		}
		catch (Exception)
		{
		}
		return result;
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

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
