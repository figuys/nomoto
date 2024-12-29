using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker;

public class ImageEntitiy : ViewModelBase
{
	private string _FilePath;

	private BitmapSource _BitmapImage;

	public ICommand ImageOperCmd { get; set; }

	public bool IsImage { get; set; }

	public string Index { get; set; }

	public string FilePath
	{
		get
		{
			return _FilePath;
		}
		set
		{
			if (!(_FilePath == value))
			{
				_FilePath = value;
				LoadImage(_FilePath);
			}
		}
	}

	public BitmapSource BitmapImage
	{
		get
		{
			return _BitmapImage;
		}
		set
		{
			_BitmapImage = value;
			OnPropertyChanged("BitmapImage");
		}
	}

	public void LoadImage(string file)
	{
		if (string.IsNullOrEmpty(file) || !File.Exists(file))
		{
			return;
		}
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.DecodePixelWidth = 250;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.UriSource = new Uri(file);
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			BitmapImage = bitmapImage;
		}
		catch (Exception)
		{
		}
	}
}
