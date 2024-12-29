using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.themes.generic.ConvertersV6;

public class TrimmedTextBlockVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		((FrameworkElement)value).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		if (((FrameworkElement)value).ActualWidth < ((FrameworkElement)value).DesiredSize.Width)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
