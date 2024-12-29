using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public class Color2BrushConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || !(value is Color))
		{
			return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#494949"));
		}
		return new SolidColorBrush((Color)value);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
