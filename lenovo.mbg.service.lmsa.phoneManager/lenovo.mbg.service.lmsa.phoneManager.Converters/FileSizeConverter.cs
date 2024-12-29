using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class FileSizeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return string.Empty;
		}
		long num = System.Convert.ToInt64(value);
		if (num == 0L)
		{
			return string.Empty;
		}
		return GlobalFun.ConvertLong2String(num);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
