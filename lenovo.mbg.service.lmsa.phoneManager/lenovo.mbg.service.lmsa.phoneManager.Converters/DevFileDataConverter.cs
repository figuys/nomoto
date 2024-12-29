using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class DevFileDataConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values == null || values.Length < 2)
		{
			return string.Empty;
		}
		if (System.Convert.ToBoolean(values[1]))
		{
			return GlobalFun.ConvertLong2String(System.Convert.ToInt64(values[0]), "F2");
		}
		return string.Empty;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
