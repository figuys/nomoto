using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.Login.Converters;

public class PasswordLevelToForegroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return new SolidColorBrush(Colors.Red);
		}
		double result = 0.0;
		double.TryParse(value.ToString(), out result);
		return (int)result switch
		{
			1 => new SolidColorBrush(Colors.Red), 
			2 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCC00")), 
			3 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00BEC1")), 
			_ => new SolidColorBrush(Colors.Red), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
