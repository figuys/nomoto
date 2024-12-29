using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.backuprestore.Converters;

public class BackupNotesTextboxTipsVisbility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Visibility visibility = Visibility.Collapsed;
		if (value == null)
		{
			return visibility;
		}
		if (value.ToString().Length >= 50)
		{
			visibility = Visibility.Visible;
		}
		return visibility;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
