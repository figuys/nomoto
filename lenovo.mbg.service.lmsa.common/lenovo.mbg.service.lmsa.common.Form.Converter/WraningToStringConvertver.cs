using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.common.Form.ViewModel;

namespace lenovo.mbg.service.lmsa.common.Form.Converter;

public class WraningToStringConvertver : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		FormItemVerifyWraningViewModel formItemVerifyWraningViewModel = (FormItemVerifyWraningViewModel)value;
		if (1.Equals(formItemVerifyWraningViewModel.WraningCode))
		{
			if (formItemVerifyWraningViewModel.WraningContent != null)
			{
				return formItemVerifyWraningViewModel.WraningContent.ToString();
			}
			return string.Empty;
		}
		return string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
