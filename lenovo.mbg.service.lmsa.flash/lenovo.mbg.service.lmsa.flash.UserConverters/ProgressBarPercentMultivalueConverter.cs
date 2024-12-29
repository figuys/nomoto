using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class ProgressBarPercentMultivalueConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length != 3)
		{
			return "0 %";
		}
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] == values)
			{
				return "0 %";
			}
		}
		double result = 0.0;
		if (!double.TryParse(values[0].ToString(), out result))
		{
			return "0 %";
		}
		double result2 = 0.0;
		if (!double.TryParse(values[1].ToString(), out result2))
		{
			return "0 %";
		}
		double result3 = 0.0;
		if (!double.TryParse(values[2].ToString(), out result3))
		{
			return "0 %";
		}
		if (result3 >= result)
		{
			return "100 %";
		}
		double num = result - result2;
		if (num == 0.0)
		{
			return "0 %";
		}
		return $"{Math.Round(result3 / num * 100.0)} %";
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
