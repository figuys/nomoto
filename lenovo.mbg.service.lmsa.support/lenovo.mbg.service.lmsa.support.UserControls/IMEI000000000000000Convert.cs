using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.support.UserControls;

public class IMEI000000000000000Convert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value.ToString().Contains("|") && value.ToString().Contains("000000000000000"))
		{
			value = value.ToString().Replace("000000000000000", "not found");
		}
		return value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
