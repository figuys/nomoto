using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class FileFolderVisibleConvert : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values != null && values[0] != null && bool.Parse(values[0].ToString()) && values[1] != null && int.Parse(values[1].ToString()) > 0)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
