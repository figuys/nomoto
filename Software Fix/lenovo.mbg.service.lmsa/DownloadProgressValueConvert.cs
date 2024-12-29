using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa;

public class DownloadProgressValueConvert : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		double num = 0.0;
		if (values == null)
		{
			return num;
		}
		try
		{
			double result = 0.0;
			double result2 = 0.0;
			double.TryParse(values[0].ToString(), out result);
			double.TryParse(values[1].ToString(), out result2);
			num = result2 * 100.0 / result;
		}
		catch (Exception)
		{
		}
		return num;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
