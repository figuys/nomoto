using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(CallType), typeof(DrawingImage))]
public class CallStatusImageConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		switch ((CallType)value)
		{
		case CallType.INCOMING:
		case CallType.VOICEMAIL:
			return (DrawingImage)LeftNavResources.SingleInstance.GetResource("receivedDrawingImage");
		case CallType.OUTGOING:
			return (DrawingImage)LeftNavResources.SingleInstance.GetResource("dialedDrawingImage");
		case CallType.MISSED:
		case CallType.REJECTED:
		case CallType.BLOCKED:
			return (DrawingImage)LeftNavResources.SingleInstance.GetResource("missedDrawingImage");
		default:
			return (DrawingImage)LeftNavResources.SingleInstance.GetResource("receivedDrawingImage");
		}
	}
}
