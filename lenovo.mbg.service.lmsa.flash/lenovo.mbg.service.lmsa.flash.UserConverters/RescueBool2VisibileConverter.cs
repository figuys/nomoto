using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.flash.UserConverters;

public class RescueBool2VisibileConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		bool flag = (bool)value;
		if (parameter != null)
		{
			string[] array = parameter.ToString().Split(':');
			if (!bool.Parse(array[0]))
			{
				_ = array[2];
			}
			else
			{
				_ = array[1];
			}
			if (!value.ToString().Equals(array[0], StringComparison.InvariantCultureIgnoreCase))
			{
				return array[2];
			}
			return array[1];
		}
		return (!flag) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
