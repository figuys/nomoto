using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.support.UserControls;

public class OpacityConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && bool.Parse(value.ToString()))
		{
			return 1;
		}
		return 0.5;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
