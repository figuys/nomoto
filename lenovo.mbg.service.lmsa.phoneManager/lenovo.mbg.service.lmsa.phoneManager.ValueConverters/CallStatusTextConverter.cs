using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(CallType), typeof(string))]
public class CallStatusTextConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (CallType)value switch
		{
			CallType.INCOMING => HostProxy.LanguageService.Translate("K0537"), 
			CallType.OUTGOING => HostProxy.LanguageService.Translate("K0539"), 
			CallType.MISSED => HostProxy.LanguageService.Translate("K0538"), 
			CallType.VOICEMAIL => HostProxy.LanguageService.Translate("Voicemail"), 
			CallType.REJECTED => HostProxy.LanguageService.Translate("Rejected"), 
			CallType.BLOCKED => HostProxy.LanguageService.Translate("Blocked"), 
			_ => HostProxy.LanguageService.Translate("K0537"), 
		};
	}
}
