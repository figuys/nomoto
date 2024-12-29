using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using lenovo.mbg.service.lmsa.support.ViewModel;

namespace lenovo.mbg.service.lmsa.support.UserControls;

public class WarrantyInfoVisbleConvert : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value is ObservableCollection<IBaseWarrantyStatusItemViewModel> && (value as ObservableCollection<IBaseWarrantyStatusItemViewModel>).Count > 0)
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
