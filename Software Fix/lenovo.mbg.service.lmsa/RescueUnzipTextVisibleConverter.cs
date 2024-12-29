using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa;

public class RescueUnzipTextVisibleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		if (DownloadStatus.UNZIPPING == (DownloadStatus)value)
		{
			return (!(parameter.ToString() == "1")) ? Visibility.Collapsed : Visibility.Visible;
		}
		return (parameter.ToString() == "1") ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
