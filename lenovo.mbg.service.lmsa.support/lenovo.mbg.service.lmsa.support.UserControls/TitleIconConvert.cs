using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.support.UserControls;

public class TitleIconConvert : IValueConverter
{
	public FrameworkElement FrameElem = new FrameworkElement();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DrawingImage result = new DrawingImage();
		if (value == null)
		{
			return result;
		}
		if (value.ToString().ToLower().Contains("charger"))
		{
			return FrameElem.TryFindResource("SupportWarrantystatusIconCharger") as DrawingImage;
		}
		if (value.ToString().ToLower().Contains("battery"))
		{
			return FrameElem.TryFindResource("SupportWarrantystatusIconBattery") as DrawingImage;
		}
		if (value.ToString().ToLower().Contains("earphone"))
		{
			return FrameElem.TryFindResource("SupportWarrantystatusIconEarphone") as DrawingImage;
		}
		return FrameElem.TryFindResource("SupportWarrantystatusIconApp") as DrawingImage;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
