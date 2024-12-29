using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.ValueConverters;

[ValueConversion(typeof(bool), typeof(DrawingImage))]
public class ContactStatusConverter : IValueConverter
{
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		int num = (int)value;
		DrawingImage result = (DrawingImage)Application.Current.Resources.MergedDictionaries[0]["TopBarDeleteImage"];
		if (num == 1)
		{
			result = (DrawingImage)Application.Current.Resources.MergedDictionaries[0]["TopBarRershImage"];
		}
		return result;
	}
}
