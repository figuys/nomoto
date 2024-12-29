using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class ConnectionTypeToImageSourceConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((Visibility)value != 0)
		{
			return (ImageSource)Application.Current.FindResource("wifiTransparentDrawingImage");
		}
		return (ImageSource)Application.Current.FindResource("usbTransparentDrawingImage");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
