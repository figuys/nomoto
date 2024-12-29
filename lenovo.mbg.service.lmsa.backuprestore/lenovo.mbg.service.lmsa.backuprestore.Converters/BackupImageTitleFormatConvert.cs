using System;
using System.Globalization;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.backuprestore.Converters;

public class BackupImageTitleFormatConvert : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		try
		{
			object arg = values[0];
			int num = (int)values[1];
			int num2 = (int)values[2];
			result = ((num2 >= 0) ? $"{arg}({num2}/{num})" : $"{arg}({num})");
		}
		catch (Exception)
		{
		}
		return result;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
