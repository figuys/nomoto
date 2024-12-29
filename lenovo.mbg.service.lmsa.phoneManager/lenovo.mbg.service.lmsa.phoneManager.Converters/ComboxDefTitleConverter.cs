using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class ComboxDefTitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length < 2)
		{
			return null;
		}
		if (values.Length == 2)
		{
			if (values[0] == null || values[0].ToString() == string.Empty)
			{
				return values[1];
			}
			return values[0];
		}
		if (values[0] == null || values[0].ToString() == string.Empty)
		{
			if (values[2] == null || values[2].ToString() == string.Empty)
			{
				return values[1];
			}
			return values[2];
		}
		if (values[2] == null || values[2].ToString() == string.Empty)
		{
			return values[0];
		}
		if (values[0].ToString().Contains(values[2].ToString()))
		{
			values[0] = null;
			return values[2];
		}
		return values[0];
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
