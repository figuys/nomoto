using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class LangConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return value;
		}
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			return HostProxy.LanguageService.Translate(value.ToString());
		}
		return value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
