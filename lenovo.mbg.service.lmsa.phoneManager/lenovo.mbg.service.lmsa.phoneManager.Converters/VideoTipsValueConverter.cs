using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class VideoTipsValueConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length != 7)
		{
			return null;
		}
		try
		{
			VideoViewType num = (VideoViewType)values[4];
			VideoViewType videoViewType = (VideoViewType)values[5];
			if (num == VideoViewType.Album && videoViewType == VideoViewType.Album && values[3] != null && int.Parse(values[3].ToString()) > 0)
			{
				return string.Format("{0} {1}", values[6], "K0647");
			}
			if (values[2] != null && int.Parse(values[2].ToString()) > 0)
			{
				return string.Format("{0} {1}", values[0], "K0649");
			}
			if (values[0] != null && int.Parse(values[0].ToString()) > 0)
			{
				return string.Format("{0} {1} ", values[0], "K0650");
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
