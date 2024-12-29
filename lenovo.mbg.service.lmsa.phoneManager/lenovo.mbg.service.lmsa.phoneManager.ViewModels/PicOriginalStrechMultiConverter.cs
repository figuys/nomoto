using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PicOriginalStrechMultiConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		BitmapSource bitmapSource = (BitmapSource)values[0];
		if (bitmapSource == null)
		{
			return Stretch.Uniform;
		}
		double num = (double)values[1];
		double num2 = (double)values[2];
		double num3 = bitmapSource.PixelWidth;
		double num4 = bitmapSource.PixelHeight;
		return (!(num3 < num2) || !(num4 < num)) ? Stretch.Uniform : Stretch.None;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
