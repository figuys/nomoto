using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public class GifVisiableConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			if (values.Length < 2)
			{
				return Visibility.Collapsed;
			}
			bool num = System.Convert.ToBoolean(values[0]);
			bool flag = System.Convert.ToBoolean(values[1]);
			return (!(num && flag)) ? Visibility.Collapsed : Visibility.Visible;
		}
		catch
		{
			return Visibility.Collapsed;
		}
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
