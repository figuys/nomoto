using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.framework.lang;

namespace lenovo.mbg.service.lmsa.Converters;

public class EmptyTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string result = LangTranslation.Translate("K0470");
		if (value != null && !string.IsNullOrEmpty(value.ToString()))
		{
			result = value.ToString();
		}
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
