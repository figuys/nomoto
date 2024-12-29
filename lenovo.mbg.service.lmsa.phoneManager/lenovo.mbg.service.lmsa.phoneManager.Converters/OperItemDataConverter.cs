using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class OperItemDataConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length < 3)
		{
			return "";
		}
		return $" ({values[1]}/{values[2]}) ";
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
