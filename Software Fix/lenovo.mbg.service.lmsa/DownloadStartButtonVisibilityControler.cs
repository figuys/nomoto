using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa;

public class DownloadStartButtonVisibilityControler : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Hidden;
		}
		DownloadStatus downloadStatus = (DownloadStatus)value;
		if (DownloadStatus.SUCCESS != downloadStatus && DownloadStatus.DOWNLOADING != downloadStatus)
		{
			return Visibility.Visible;
		}
		return Visibility.Hidden;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value;
	}
}
