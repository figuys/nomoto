using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class SmartDeviceImageConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && (bool)value)
		{
			return new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/icSmartdevice-check.png"));
		}
		return new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/icSmartdevice-un-check.png"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
