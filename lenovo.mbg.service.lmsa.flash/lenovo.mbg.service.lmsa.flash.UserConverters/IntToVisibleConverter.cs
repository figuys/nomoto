using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class IntToVisibleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool flag = true;
		if (value != null && "-1" == value.ToString())
		{
			flag = false;
		}
		return flag;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
