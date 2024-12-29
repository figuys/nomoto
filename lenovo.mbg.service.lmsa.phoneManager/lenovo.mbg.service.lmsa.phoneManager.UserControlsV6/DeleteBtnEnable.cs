using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public class DeleteBtnEnable : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && int.Parse(value.ToString()) > 0)
		{
			return true;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
