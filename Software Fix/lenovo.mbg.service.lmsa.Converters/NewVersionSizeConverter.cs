using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadMode;
using lenovo.mbg.service.framework.lang;

namespace lenovo.mbg.service.lmsa.Converters;

public class NewVersionSizeConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		try
		{
			string text = "";
			long filesize = (long)values[0];
			if (filesize == 0L && values[1] != null)
			{
				string url = values[1].ToString();
				GlobalFun.GetFileSize(url, out filesize);
			}
			text = GlobalFun.ConvertLong2String(filesize);
			result = LangTranslation.Translate(values[2].ToString()) + text;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		try
		{
			string arg = "";
			long num = (long)value;
			if (num == 0)
			{
				HttpDownload httpDownload = new HttpDownload();
			}
			if ((double)num < 1024.0)
			{
				arg = num.ToString("F2") + " Byte";
			}
			else if ((double)num >= 1024.0 && num < 1048576)
			{
				arg = ((double)num / 1024.0).ToString("F2") + " K";
			}
			else if (num >= 1048576 && num < 1073741824)
			{
				arg = ((double)num / 1024.0 / 1024.0).ToString("F2") + " M";
			}
			else if (num >= 1073741824)
			{
				arg = ((double)num / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " G";
			}
			result = string.Format("{1}:{0}", arg, LangTranslation.Translate("K0378"));
		}
		catch (Exception)
		{
		}
		return result;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
