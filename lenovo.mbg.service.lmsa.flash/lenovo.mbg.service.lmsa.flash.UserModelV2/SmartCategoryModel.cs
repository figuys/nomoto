using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class SmartCategoryModel
{
	public int id { get; set; }

	public string name { get; set; }

	public string imgPath { get; set; }

	public List<SmartMarketNameModel> marketNames { get; set; }

	public ImageSource Image
	{
		get
		{
			if (!string.IsNullOrEmpty(imgPath))
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(imgPath, UriKind.Absolute);
				bitmapImage.EndInit();
				return bitmapImage;
			}
			return null;
		}
	}
}
