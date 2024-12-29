using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Comm;

public class MultiBoolConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length < 2)
		{
			return false;
		}
		for (int i = 0; i < values.Length; i++)
		{
			if (!System.Convert.ToBoolean(values[i]))
			{
				return false;
			}
		}
		return true;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
