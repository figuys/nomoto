using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class DetailButtonVisibleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Visibility visibility = Visibility.Visible;
		if (value != null && "-1" == value.ToString())
		{
			visibility = Visibility.Collapsed;
		}
		return visibility;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
