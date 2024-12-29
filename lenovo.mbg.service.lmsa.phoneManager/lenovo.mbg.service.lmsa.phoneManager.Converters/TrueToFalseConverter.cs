using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public sealed class TrueToFalseConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return !(bool)value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
