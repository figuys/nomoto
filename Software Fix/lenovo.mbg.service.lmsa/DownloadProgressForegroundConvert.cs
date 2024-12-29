using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa;

public class DownloadProgressForegroundConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
