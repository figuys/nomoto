using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public class LangStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value as string;
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		int result = 30;
		if (parameter == null)
		{
			result = 30;
		}
		else
		{
			int.TryParse(parameter.ToString(), out result);
		}
		if (text.Length > result)
		{
			return text.Substring(0, result) + "...";
		}
		return text;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
