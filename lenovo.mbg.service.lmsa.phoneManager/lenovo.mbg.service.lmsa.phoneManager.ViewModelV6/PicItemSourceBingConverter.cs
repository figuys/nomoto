using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicItemSourceBingConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[1] == null)
		{
			return null;
		}
		string text = parameter.ToString();
		if (!(text == "grouped"))
		{
			if (text == "notGrouped")
			{
				((PicMgtViewModelV7)values[1]).PicNotGroupScrollViewer = (ScrollViewer)values[2];
			}
		}
		else
		{
			((PicMgtViewModelV7)values[1]).DateGroupScrollViewer = (ScrollViewer)values[2];
		}
		return values[0];
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
