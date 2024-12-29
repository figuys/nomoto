using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.backuprestore.Converters;

public class BackupRestoreChooseImangeConverters : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		BitmapImage result = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;component/Resources/Images/not-choose.png"));
		try
		{
			if ((bool)value)
			{
				result = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;component/Resources/Images/choose.png"));
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
