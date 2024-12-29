using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.themes.generic.Converter;

public class CategoryForcgroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B9B9B"));
		}
		int result = 0;
		int.TryParse(value.ToString(), out result);
		if (result > 0)
		{
			return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		}
		return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B9B9B"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
