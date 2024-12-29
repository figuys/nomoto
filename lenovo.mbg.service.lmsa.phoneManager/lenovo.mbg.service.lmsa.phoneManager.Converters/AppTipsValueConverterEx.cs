using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class AppTipsValueConverterEx : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null)
		{
			return null;
		}
		try
		{
			if (values[1] != null && int.Parse(values[1].ToString()) > 0)
			{
				return values[1].ToString();
			}
			return values[0].ToString();
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
