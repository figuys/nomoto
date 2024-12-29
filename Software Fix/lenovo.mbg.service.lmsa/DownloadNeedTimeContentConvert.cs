using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa;

public class DownloadNeedTimeContentConvert : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string result = "--";
		if (values == null)
		{
			return result;
		}
		try
		{
			DownloadStatus downloadStatus = (DownloadStatus)values[0];
			if (downloadStatus == DownloadStatus.MANUAL_PAUSE || downloadStatus == DownloadStatus.AUTO_PAUSE)
			{
				return LangTranslation.Translate(values[2].ToString());
			}
			if (downloadStatus == DownloadStatus.UNZIPPING)
			{
				return null;
			}
			if (values[1] == null || string.IsNullOrEmpty(values[1].ToString()))
			{
				return result;
			}
			return values[1].ToString();
		}
		catch (Exception)
		{
			return result;
		}
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
