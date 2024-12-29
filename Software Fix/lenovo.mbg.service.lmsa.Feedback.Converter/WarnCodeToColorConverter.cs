using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.common.Form.ViewModel;

namespace lenovo.mbg.service.lmsa.Feedback.Converter;

public class WarnCodeToColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Application.Current.TryFindResource("V6_BorderBrushKey") as SolidColorBrush;
		}
		FormItemVerifyWraningViewModel formItemVerifyWraningViewModel = (FormItemVerifyWraningViewModel)value;
		if (1.Equals(formItemVerifyWraningViewModel.WraningCode) && formItemVerifyWraningViewModel.WraningContent != null)
		{
			return Application.Current.TryFindResource("V6_WarnningBrushKey") as SolidColorBrush;
		}
		return Application.Current.TryFindResource("V6_BorderBrushKey") as SolidColorBrush;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
