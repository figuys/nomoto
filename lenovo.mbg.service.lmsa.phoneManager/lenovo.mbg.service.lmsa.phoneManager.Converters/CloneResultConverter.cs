using System;
using System.Globalization;
using System.Windows.Data;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.phoneManager.ModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.Converters;

public class CloneResultConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || !(value is OperResultModel))
		{
			return string.Empty;
		}
		OperResultModel operResultModel = value as OperResultModel;
		int num = operResultModel.Total - operResultModel.Complete;
		if (!operResultModel.IsComplete)
		{
			return string.Format("{0} {1}", num, LangTranslation.Translate("K0337"));
		}
		return LangTranslation.Translate("K0638");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
