using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

public class ImageHandleHelper
{
	public static BitmapImage LoadBitmap(string fileName)
	{
		if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
		{
			return null;
		}
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.UriSource = new Uri(fileName, UriKind.Absolute);
			bitmapImage.EndInit();
			return bitmapImage;
		}
		catch
		{
			return null;
		}
	}
}
