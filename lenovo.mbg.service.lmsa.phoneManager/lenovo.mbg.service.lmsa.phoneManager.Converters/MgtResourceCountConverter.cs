using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class MgtResourceCountConverter : IMultiValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value;
	}

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		try
		{
			result = $"{values[0]}";
		}
		catch (Exception)
		{
		}
		return result;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
