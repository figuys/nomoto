using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(ContactType), typeof(DrawingImage))]
public class CallSimUsedConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DrawingImage result = null;
		switch ((ContactType)value)
		{
		case ContactType.PHONE:
			result = (DrawingImage)LeftNavResources.SingleInstance.GetResource("camera_rollDrawingImage");
			break;
		case ContactType.SIM1:
			result = (DrawingImage)LeftNavResources.SingleInstance.GetResource("sim1DrawingImage");
			break;
		case ContactType.SIM2:
			result = (DrawingImage)LeftNavResources.SingleInstance.GetResource("sim2DrawingImage");
			break;
		}
		return result;
	}
}
