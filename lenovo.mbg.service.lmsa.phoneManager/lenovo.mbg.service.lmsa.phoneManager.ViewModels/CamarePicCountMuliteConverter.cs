using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class CamarePicCountMuliteConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string result = "0";
		if (values == null || values.Count() != 2)
		{
			return result;
		}
		object obj = values[0];
		object obj2 = values[1];
		if (obj == null)
		{
			return result;
		}
		return obj2.ToString();
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
