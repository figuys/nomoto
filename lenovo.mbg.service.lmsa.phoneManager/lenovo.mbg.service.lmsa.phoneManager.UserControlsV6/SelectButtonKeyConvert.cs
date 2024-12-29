using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public class SelectButtonKeyConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && bool.Parse(value.ToString()))
		{
			return "K1774";
		}
		return "K0764";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
