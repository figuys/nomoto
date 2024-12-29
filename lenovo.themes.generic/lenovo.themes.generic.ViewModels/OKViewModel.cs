using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class OKViewModel : ViewModelBase
{
	private Brush _background;

	private Visibility _showLine = Visibility.Hidden;

	private double _opacity = double.NaN;

	private ImageSource _TitleIconImageSource;

	private ImageSource _contentIconImageSource;

	private string _title = string.Empty;

	private double _titleFontSize = 6.0;

	private Brush _titleForeground;

	private string _content = string.Empty;

	private Visibility _WarnIconVisibility = Visibility.Collapsed;

	private double _contentFontSize = 6.0;

	private Brush _contentForeground;

	private string _okButtonText = string.Empty;

	private double _okButtonTextFontSize = 6.0;

	private Brush _OKButtonBackground;

	private Brush _okButtonMouseOverBackground;

	private Brush _okButtonForeground;

	private string countDownTip;

	private volatile int mCountdownSeconds;

	private Timer mCountdownTimer;

	public Brush Background
	{
		get
		{
			return _background;
		}
		set
		{
			if (_background != value)
			{
				_background = value;
				OnPropertyChanged("Background");
			}
		}
	}

	public Visibility ShowLine
	{
		get
		{
			return _showLine;
		}
		set
		{
			if (_showLine != value)
			{
				_showLine = value;
				OnPropertyChanged("ShowLine");
			}
		}
	}

	public double Opacity
	{
		get
		{
			return _opacity;
		}
		set
		{
			if (_opacity != value)
			{
				_opacity = value;
				OnPropertyChanged("Opacity");
			}
		}
	}

	public ImageSource TitleIconImageSource
	{
		get
		{
			return _TitleIconImageSource;
		}
		set
		{
			_TitleIconImageSource = value;
			OnPropertyChanged("TitleIconImageSource");
		}
	}

	public ImageSource ContentIconImageSource
	{
		get
		{
			return _contentIconImageSource;
		}
		set
		{
			_contentIconImageSource = value;
			OnPropertyChanged("ContentIconImageSource");
		}
	}

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			_title = value;
			OnPropertyChanged("Title");
		}
	}

	public double TitleFontSize
	{
		get
		{
			return _titleFontSize;
		}
		set
		{
			_titleFontSize = value;
			OnPropertyChanged("TitleFontSize");
		}
	}

	public Brush TitleForeground
	{
		get
		{
			return _titleForeground;
		}
		set
		{
			_titleForeground = value;
			OnPropertyChanged("TitleForeground");
		}
	}

	public string Content
	{
		get
		{
			return _content;
		}
		set
		{
			_content = value;
			OnPropertyChanged("Content");
		}
	}

	public Visibility WarnIconVisibility
	{
		get
		{
			return _WarnIconVisibility;
		}
		set
		{
			_WarnIconVisibility = value;
			OnPropertyChanged("WarnIconVisibility");
		}
	}

	public double ContentFontSize
	{
		get
		{
			return _contentFontSize;
		}
		set
		{
			_contentFontSize = value;
			OnPropertyChanged("ContentFontSize");
		}
	}

	public Brush ContentForeground
	{
		get
		{
			return _contentForeground;
		}
		set
		{
			_contentForeground = value;
			OnPropertyChanged("ContentForeground");
		}
	}

	public string OKButtonText
	{
		get
		{
			return _okButtonText;
		}
		set
		{
			_okButtonText = value;
			OnPropertyChanged("OKButtonText");
		}
	}

	public double OKButtonTextFontSize
	{
		get
		{
			return _okButtonTextFontSize;
		}
		set
		{
			_okButtonTextFontSize = value;
			OnPropertyChanged("OKButtonTextFontSize");
		}
	}

	public Brush OKButtonBackground
	{
		get
		{
			return _OKButtonBackground;
		}
		set
		{
			_OKButtonBackground = value;
			OnPropertyChanged("OKButtonBackground");
		}
	}

	public Brush OKButtonMouseOverBackground
	{
		get
		{
			return _okButtonMouseOverBackground;
		}
		set
		{
			if (_okButtonMouseOverBackground != value)
			{
				_okButtonMouseOverBackground = value;
				OnPropertyChanged("OKButtonMouseOverBackground");
			}
		}
	}

	public Brush OKButtonForeground
	{
		get
		{
			return _okButtonForeground;
		}
		set
		{
			_okButtonForeground = value;
			OnPropertyChanged("OKButtonForeground");
		}
	}

	public ReplayCommand OKButtonClickCommand { get; set; }

	public string CountDownTip
	{
		get
		{
			return countDownTip;
		}
		set
		{
			if (!(countDownTip == value))
			{
				countDownTip = value;
				OnPropertyChanged("CountDownTip");
			}
		}
	}

	protected virtual void OKButtonClickCommandHandler(object parameter)
	{
		if (parameter is Window window)
		{
			window.Close();
		}
	}

	public void Countdown(int seconds, Action callback)
	{
		if (seconds <= 0)
		{
			return;
		}
		mCountdownSeconds = seconds;
		mCountdownTimer = new Timer(delegate
		{
			CountDownTip = $"({mCountdownSeconds})";
			if (mCountdownSeconds <= 0)
			{
				CountDownTip = string.Empty;
				mCountdownTimer?.Dispose();
				mCountdownTimer = null;
				callback?.Invoke();
			}
			mCountdownSeconds--;
		}, null, 0, 1000);
	}

	public OKViewModel()
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		TitleFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("12pt"));
		TitleForeground = new SolidColorBrush(Color.FromRgb(143, 143, 143));
		ContentFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"));
		ContentForeground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
		OKButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		OKButtonMouseOverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		OKButtonForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
		OKButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"));
		OKButtonClickCommand = new ReplayCommand(OKButtonClickCommandHandler);
	}

	public static OKViewModel DefaultValues()
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		return new OKViewModel
		{
			TitleIconImageSource = (ComponentResources.SingleInstance.GetResource("drawingImage_warning") as ImageSource),
			TitleFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("12pt")),
			TitleForeground = new SolidColorBrush(Color.FromRgb(73, 73, 73)),
			ContentFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt")),
			ContentForeground = new SolidColorBrush(Color.FromRgb(119, 119, 118)),
			OKButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2")),
			OKButtonMouseOverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2")),
			OKButtonForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")),
			OKButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"))
		};
	}
}
