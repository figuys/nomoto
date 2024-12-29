using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public class ZoomConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length != 2)
		{
			return "Auto";
		}
		double num = System.Convert.ToDouble(values[0]);
		double num2 = System.Convert.ToDouble(values[1]);
		return num * num2;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
