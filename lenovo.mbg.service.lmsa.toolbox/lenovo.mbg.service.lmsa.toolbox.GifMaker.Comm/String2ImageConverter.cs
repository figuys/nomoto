using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public class String2ImageConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value as string;
		if (string.IsNullOrEmpty(text) || !File.Exists(text))
		{
			return null;
		}
		return new BitmapImage(new Uri(text, UriKind.RelativeOrAbsolute));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
