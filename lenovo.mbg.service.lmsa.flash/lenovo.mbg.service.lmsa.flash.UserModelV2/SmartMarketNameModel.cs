using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class SmartMarketNameModel
{
	[JsonProperty("marketName")]
	public string MarketName { get; set; }

	public string marketImgPath { get; set; }

	[JsonProperty("readSupport")]
	public bool ReadSupport { get; set; }

	public ImageSource Image
	{
		get
		{
			if (!string.IsNullOrEmpty(marketImgPath))
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(marketImgPath, UriKind.Absolute);
				bitmapImage.EndInit();
				return bitmapImage;
			}
			return null;
		}
	}
}
