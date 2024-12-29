using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class AppTipsValueConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null)
		{
			return null;
		}
		try
		{
			if (values.Length == 3 && values[2] != null && int.Parse(values[2].ToString()) > 0)
			{
				return string.Format("{0} {1}", values[0], "K0645");
			}
			if (values[0] != null && int.Parse(values[0].ToString()) > 0)
			{
				return string.Format("{0} {1}", values[0], "K0646");
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
