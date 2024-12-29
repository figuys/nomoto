using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.Converters;

public class CompareRedioBtnConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length != 2)
		{
			return false;
		}
		if (int.TryParse(values[0].ToString(), out var result) && int.TryParse(values[1].ToString(), out var result2) && result >= result2)
		{
			return true;
		}
		return false;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
