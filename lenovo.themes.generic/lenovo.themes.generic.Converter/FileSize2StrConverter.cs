using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.Converter;

public class FileSize2StrConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return GlobalFun.ConvertLong2String((long)value);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
