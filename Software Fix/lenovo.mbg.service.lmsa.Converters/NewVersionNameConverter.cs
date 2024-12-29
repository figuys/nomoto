using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.framework.lang;

namespace lenovo.mbg.service.lmsa.Converters;

public class NewVersionNameConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		try
		{
			string text = (string)value;
			result = ((!string.Equals(text, "Client", StringComparison.CurrentCultureIgnoreCase)) ? string.Format("{1} ({0})", LangTranslation.Translate(text), LangTranslation.Translate("Plugin Upgrade")) : "K0341");
		}
		catch (Exception)
		{
		}
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
