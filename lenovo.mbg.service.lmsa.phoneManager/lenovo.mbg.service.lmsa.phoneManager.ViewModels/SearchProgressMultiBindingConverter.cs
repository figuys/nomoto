using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class SearchProgressMultiBindingConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length != 3)
		{
			return Visibility.Hidden;
		}
		object obj = null;
		if (values[0] != null && values[1] != null && (obj = values[2]) != null)
		{
			bool result = false;
			if (bool.TryParse(obj.ToString(), out result) && result)
			{
				return Visibility.Visible;
			}
		}
		return Visibility.Hidden;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
