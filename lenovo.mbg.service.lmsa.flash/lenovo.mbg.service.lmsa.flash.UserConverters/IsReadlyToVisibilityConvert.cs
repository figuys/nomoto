using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public sealed class IsReadlyToVisibilityConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			bool flag = (bool)value;
			string[] array = parameter.ToString().Split(':');
			return (bool.Parse(array[0]) == flag) ? array[1] : array[2];
		}
		catch (Exception)
		{
			return Visibility.Collapsed;
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
