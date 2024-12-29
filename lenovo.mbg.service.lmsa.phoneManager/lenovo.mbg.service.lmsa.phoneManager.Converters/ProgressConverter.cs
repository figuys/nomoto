using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class ProgressConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length != 3)
		{
			return 0.0;
		}
		try
		{
			double num = System.Convert.ToDouble(values[0]);
			double num2 = System.Convert.ToDouble(values[1]);
			double num3 = System.Convert.ToDouble(values[2]);
			return num * num2 / num3;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
