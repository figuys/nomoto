using System;
using System.IO;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;

public class GifMakerModel : ViewModelBase
{
	private int _delay;

	private int _GifHeight;

	private int _GifWidth;

	private string _GifPath;

	private bool _IsAnimation;

	private bool _IsGifMakeOk;

	private BitmapImage _GifImage;

	public int GifDelay
	{
		get
		{
			return _delay;
		}
		set
		{
			if (_delay != value)
			{
				_delay = value;
				IsAnimation = false;
				IsChanged = true;
				OnPropertyChanged("GifDelay");
			}
		}
	}

	public int GifHeight
	{
		get
		{
			return _GifHeight;
		}
		set
		{
			if (_GifHeight != value)
			{
				_GifHeight = value;
				IsAnimation = false;
				IsChanged = true;
				OnPropertyChanged("GifHeight");
			}
		}
	}

	public int GifWidth
	{
		get
		{
			return _GifWidth;
		}
		set
		{
			if (_GifWidth != value)
			{
				_GifWidth = value;
				IsAnimation = false;
				IsChanged = true;
				OnPropertyChanged("GifWidth");
			}
		}
	}

	public string GifPath
	{
		get
		{
			return _GifPath;
		}
		set
		{
			_GifPath = value;
			IsAnimation = false;
			OnPropertyChanged("GifPath");
			Configurations.GifSavePath = _GifPath;
		}
	}

	public bool IsAnimation
	{
		get
		{
			return _IsAnimation;
		}
		set
		{
			if (_IsAnimation != value)
			{
				_IsAnimation = value;
				if (!_IsAnimation)
				{
					IsGifMakeOk = false;
				}
				OnPropertyChanged("IsAnimation");
			}
		}
	}

	public bool IsGifMakeOk
	{
		get
		{
			return _IsGifMakeOk;
		}
		set
		{
			if (_IsGifMakeOk != value)
			{
				_IsGifMakeOk = value;
				OnPropertyChanged("IsGifMakeOk");
			}
		}
	}

	public bool IsChanged { get; set; }

	public string TempDir { get; private set; }

	public string GifFile { get; private set; }

	public BitmapImage GifImage
	{
		get
		{
			return _GifImage;
		}
		set
		{
			_GifImage = value;
			OnPropertyChanged("GifImage");
		}
	}

	public GifMakerModel()
	{
		GifImage = null;
		IsChanged = false;
		GifDelay = 4;
		GifHeight = 200;
		GifWidth = 200;
		IsGifMakeOk = false;
		IsAnimation = false;
		TempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
		GifPath = Configurations.GifSavePath;
		GifFile = Path.Combine(TempDir, "temp.gif");
	}

	public void LoadBitmap()
	{
		if (File.Exists(GifFile))
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = File.Open(GifFile, FileMode.Open);
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			GifImage = bitmapImage;
		}
	}
}
