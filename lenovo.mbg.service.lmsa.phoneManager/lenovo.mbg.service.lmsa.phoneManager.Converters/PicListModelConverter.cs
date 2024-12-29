using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class PicListModelConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		if (!Enum.TryParse<PicMgtContentViewDisplayMode>(value.ToString(), out var result))
		{
			return false;
		}
		string text = parameter?.ToString();
		if (text == "1" && result == PicMgtContentViewDisplayMode.PicListWithDateGroup)
		{
			return true;
		}
		if (text == "2" && result == PicMgtContentViewDisplayMode.PicListWithFlowWater)
		{
			return true;
		}
		if (text == "3" && result == PicMgtContentViewDisplayMode.PicDetailLisst)
		{
			return true;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = parameter?.ToString();
		if (value != null && (bool)value)
		{
			if (text == "1")
			{
				return PicMgtContentViewDisplayMode.PicListWithDateGroup;
			}
			if (text == "2")
			{
				return PicMgtContentViewDisplayMode.PicListWithFlowWater;
			}
			return PicMgtContentViewDisplayMode.PicDetailLisst;
		}
		return Binding.DoNothing;
	}
}
