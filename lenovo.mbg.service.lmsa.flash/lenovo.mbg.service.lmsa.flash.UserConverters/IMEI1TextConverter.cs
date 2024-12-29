using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class IMEI1TextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string result = "K0459";
		if (value != null && !string.IsNullOrEmpty(value.ToString()))
		{
			result = "K0460";
		}
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
