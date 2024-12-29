using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(bool), typeof(DrawingImage))]
public class PhoneCardsConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (DrawingImage)Application.Current.Resources.MergedDictionaries[0]["sim1DrawingImage"];
	}
}
