using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class CloneFialedConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			if (values == null || values.Length < 2)
			{
				return "0";
			}
			int num = System.Convert.ToInt32(values[0]);
			int num2 = System.Convert.ToInt32(values[1]);
			return (num - num2).ToString();
		}
		catch
		{
			return "0";
		}
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
