using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa;

public class DownloadStatus2ImageConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && (DownloadStatus)value == DownloadStatus.DOWNLOADING)
		{
			return Application.Current.FindResource("suspendDrawingImage");
		}
		return Application.Current.FindResource("startDrawingImage");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
