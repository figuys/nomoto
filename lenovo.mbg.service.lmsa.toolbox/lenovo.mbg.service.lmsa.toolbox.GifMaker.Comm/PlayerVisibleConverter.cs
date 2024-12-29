using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public class PlayerVisibleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			if (values == null || values.Length < 2)
			{
				return Visibility.Collapsed;
			}
			string text = values[0].ToString();
			uint num = System.Convert.ToUInt32(values[1]);
			return (!(text == "True") || num < 2) ? Visibility.Collapsed : Visibility.Visible;
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
