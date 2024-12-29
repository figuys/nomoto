using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa;

public class DownloadInfoNormalVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Visibility visibility = Visibility.Visible;
		if (value == null)
		{
			return visibility;
		}
		string value2 = value.ToString();
		if (!string.IsNullOrEmpty(value2))
		{
			visibility = Visibility.Collapsed;
		}
		return visibility;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
