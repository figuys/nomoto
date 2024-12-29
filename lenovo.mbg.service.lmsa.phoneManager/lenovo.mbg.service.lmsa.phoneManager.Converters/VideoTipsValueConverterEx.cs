using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class VideoTipsValueConverterEx : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null)
		{
			return null;
		}
		try
		{
			VideoViewType num = (VideoViewType)values[3];
			VideoViewType videoViewType = (VideoViewType)values[4];
			if (num == VideoViewType.Album && videoViewType == VideoViewType.Album && values[2] != null && int.Parse(values[2].ToString()) > 0)
			{
				return values[2].ToString();
			}
			if (values[1] != null && int.Parse(values[1].ToString()) > 0)
			{
				return values[1].ToString();
			}
			if (values[5] != null && int.Parse(values[5].ToString()) > 0)
			{
				return values[0].ToString();
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
