using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.backuprestore.Converters;

public class CategoryImageConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string[] array = parameter.ToString().Split(':');
		if (value == null)
		{
			return array[2];
		}
		int result = 0;
		bool flag = bool.Parse(array[0]);
		int.TryParse(value.ToString(), out result);
		if (result > 0 == flag)
		{
			return array[1];
		}
		return array[2];
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
