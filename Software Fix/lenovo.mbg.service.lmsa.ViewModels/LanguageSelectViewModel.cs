using System;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class LanguageSelectViewModel : OKCancelViewModel
{
	public enum CommandResult
	{
		Cancel,
		RestartNow,
		RestartLater
	}

	private string _readyLangTipContent = string.Empty;

	private double _readyLangTipContentFontSize = 6.0;

	private Brush _readyLangTipContentForeground = null;

	private bool _language1Checked = false;

	private bool _language2Checked = false;

	private bool _language3Checked = false;

	private bool _language4Checked = false;

	private bool _language5Checked = false;

	private bool _language6Checked = false;

	private bool _language7Checked = false;

	private bool _language8Checked = false;

	private bool _language9Checked = false;

	private bool _language10Checked = false;

	private bool _language11Checked = false;

	private bool _language12Checked = false;

	private bool _language13Checked = false;

	private bool _language14Checked = false;

	private bool _language15Checked = false;

	private string _languageSelected = string.Empty;

	private string _restartNowButtonText = string.Empty;

	private double _restartNowButtonTextFontSize = 6.0;

	private Brush _restartNowButtonBackground;

	private Brush _restartNowButtonForeground;

	private Brush _restartNowButtonMouseOverBackground;

	private Brush _restartNowButtonDisabledBackground;

	private string _restartLaterButtonText = string.Empty;

	private double _restartLaterButtonTextFontSize = 6.0;

	private Brush _restartLaterButtonBackground;

	private Brush _restartLaterButtonForeground;

	private Brush _restartLaterButtonMouseOverBackground;

	private Brush _restartLaterButtonDisabledBackground;

	public string ReadyLangTipContent
	{
		get
		{
			return _readyLangTipContent;
		}
		set
		{
			_readyLangTipContent = value;
			OnPropertyChanged("ReadyLangTipContent");
		}
	}

	public double ReadyLangTipContentFontSize
	{
		get
		{
			return _readyLangTipContentFontSize;
		}
		set
		{
			_readyLangTipContentFontSize = value;
			OnPropertyChanged("ReadyLangTipContentFontSize");
		}
	}

	public Brush ReadyLangTipContentForeground
	{
		get
		{
			return _readyLangTipContentForeground;
		}
		set
		{
			_readyLangTipContentForeground = value;
			OnPropertyChanged("ReadyLangTipContentForeground");
		}
	}

	public bool Language1Checked
	{
		get
		{
			return _language1Checked;
		}
		set
		{
			_language1Checked = value;
			if (_language1Checked)
			{
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language1Checked");
		}
	}

	public bool Language2Checked
	{
		get
		{
			return _language2Checked;
		}
		set
		{
			_language2Checked = value;
			if (_language2Checked)
			{
				Language1Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language2Checked");
		}
	}

	public bool Language3Checked
	{
		get
		{
			return _language3Checked;
		}
		set
		{
			_language3Checked = value;
			if (_language3Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language3Checked");
		}
	}

	public bool Language4Checked
	{
		get
		{
			return _language4Checked;
		}
		set
		{
			_language4Checked = value;
			if (_language4Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language4Checked");
		}
	}

	public bool Language5Checked
	{
		get
		{
			return _language5Checked;
		}
		set
		{
			_language5Checked = value;
			if (_language5Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language5Checked");
		}
	}

	public bool Language6Checked
	{
		get
		{
			return _language6Checked;
		}
		set
		{
			_language6Checked = value;
			if (_language6Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language6Checked");
		}
	}

	public bool Language7Checked
	{
		get
		{
			return _language7Checked;
		}
		set
		{
			_language7Checked = value;
			if (_language7Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language7Checked");
		}
	}

	public bool Language8Checked
	{
		get
		{
			return _language8Checked;
		}
		set
		{
			_language8Checked = value;
			if (_language8Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language8Checked");
		}
	}

	public bool Language9Checked
	{
		get
		{
			return _language9Checked;
		}
		set
		{
			_language9Checked = value;
			if (_language9Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language9Checked");
		}
	}

	public bool Language10Checked
	{
		get
		{
			return _language10Checked;
		}
		set
		{
			_language10Checked = value;
			if (_language10Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language10Checked");
		}
	}

	public bool Language11Checked
	{
		get
		{
			return _language11Checked;
		}
		set
		{
			_language11Checked = value;
			if (_language11Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language11Checked");
		}
	}

	public bool Language12Checked
	{
		get
		{
			return _language12Checked;
		}
		set
		{
			_language12Checked = value;
			if (_language12Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language13Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language12Checked");
		}
	}

	public bool Language13Checked
	{
		get
		{
			return _language13Checked;
		}
		set
		{
			_language13Checked = value;
			if (_language13Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language14Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language13Checked");
		}
	}

	public bool Language14Checked
	{
		get
		{
			return _language14Checked;
		}
		set
		{
			_language14Checked = value;
			if (_language14Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language15Checked = false;
			}
			OnPropertyChanged("Language14Checked");
		}
	}

	public bool Language15Checked
	{
		get
		{
			return _language15Checked;
		}
		set
		{
			_language15Checked = value;
			if (_language15Checked)
			{
				Language1Checked = false;
				Language2Checked = false;
				Language3Checked = false;
				Language4Checked = false;
				Language5Checked = false;
				Language6Checked = false;
				Language7Checked = false;
				Language8Checked = false;
				Language9Checked = false;
				Language10Checked = false;
				Language11Checked = false;
				Language12Checked = false;
				Language13Checked = false;
				Language14Checked = false;
			}
			OnPropertyChanged("Language15Checked");
		}
	}

	public string LanguageSelected
	{
		get
		{
			return _languageSelected;
		}
		set
		{
			_languageSelected = value;
			OnPropertyChanged("LanguageSelected");
		}
	}

	public CommandResult ButtonCommandResult { get; set; }

	public ReplayCommand SelectLanguageClickCommand { get; set; }

	public string RestartNowButtonText
	{
		get
		{
			return _restartNowButtonText;
		}
		set
		{
			_restartNowButtonText = value;
			OnPropertyChanged("RestartNowButtonText");
		}
	}

	public double RestartNowButtonTextFontSize
	{
		get
		{
			return _restartNowButtonTextFontSize;
		}
		set
		{
			_restartNowButtonTextFontSize = value;
			OnPropertyChanged("RestartNowButtonTextFontSize");
		}
	}

	public Brush RestartNowButtonBackground
	{
		get
		{
			return _restartNowButtonBackground;
		}
		set
		{
			_restartNowButtonBackground = value;
			OnPropertyChanged("RestartNowButtonBackground");
		}
	}

	public Brush RestartNowButtonForeground
	{
		get
		{
			return _restartNowButtonForeground;
		}
		set
		{
			_restartNowButtonForeground = value;
			OnPropertyChanged("RestartNowButtonForeground");
		}
	}

	public Brush RestartNowButtonMouseOverBackground
	{
		get
		{
			return _restartNowButtonMouseOverBackground;
		}
		set
		{
			if (_restartNowButtonMouseOverBackground != value)
			{
				_restartNowButtonMouseOverBackground = value;
				OnPropertyChanged("RestartNowButtonMouseOverBackground");
			}
		}
	}

	public Brush RestartNowButtonDisabledBackground
	{
		get
		{
			return _restartNowButtonDisabledBackground;
		}
		set
		{
			if (_restartNowButtonDisabledBackground != value)
			{
				_restartNowButtonDisabledBackground = value;
				OnPropertyChanged("RestartNowButtonDisabledBackground");
			}
		}
	}

	public ReplayCommand RestartNowButtonClickCommand { get; set; }

	public string RestartLaterButtonText
	{
		get
		{
			return _restartLaterButtonText;
		}
		set
		{
			_restartLaterButtonText = value;
			OnPropertyChanged("RestartLaterButtonText");
		}
	}

	public double RestartLaterButtonTextFontSize
	{
		get
		{
			return _restartLaterButtonTextFontSize;
		}
		set
		{
			_restartLaterButtonTextFontSize = value;
			OnPropertyChanged("RestartLaterButtonTextFontSize");
		}
	}

	public Brush RestartLaterButtonBackground
	{
		get
		{
			return _restartLaterButtonBackground;
		}
		set
		{
			_restartLaterButtonBackground = value;
			OnPropertyChanged("RestartLaterButtonBackground");
		}
	}

	public Brush RestartLaterButtonForeground
	{
		get
		{
			return _restartLaterButtonForeground;
		}
		set
		{
			_restartLaterButtonForeground = value;
			OnPropertyChanged("RestartLaterButtonForeground");
		}
	}

	public Brush RestartLaterButtonMouseOverBackground
	{
		get
		{
			return _restartLaterButtonMouseOverBackground;
		}
		set
		{
			if (_restartLaterButtonMouseOverBackground != value)
			{
				_restartLaterButtonMouseOverBackground = value;
				OnPropertyChanged("RestartLaterButtonMouseOverBackground");
			}
		}
	}

	public Brush RestartLaterButtonDisabledBackground
	{
		get
		{
			return _restartLaterButtonDisabledBackground;
		}
		set
		{
			if (_restartLaterButtonDisabledBackground != value)
			{
				_restartLaterButtonDisabledBackground = value;
				OnPropertyChanged("RestartLaterButtonDisabledBackground");
			}
		}
	}

	public ReplayCommand RestartLaterButtonClickCommand { get; set; }

	public LanguageSelectViewModel()
	{
		SelectLanguageClickCommand = new ReplayCommand(SelectLanguageClickCommandHandler);
		RestartNowButtonClickCommand = new ReplayCommand(RestartNowClickCommandHandler);
		RestartLaterButtonClickCommand = new ReplayCommand(RestartLaterClickCommandHandler);
	}

	protected virtual void SelectLanguageClickCommandHandler(object parameter)
	{
		LanguageSelected = parameter as string;
	}

	private void RestartNowClickCommandHandler(object parameter)
	{
		if (parameter is Window window)
		{
			window.Close();
		}
		ButtonCommandResult = CommandResult.RestartNow;
	}

	private void RestartLaterClickCommandHandler(object parameter)
	{
		if (parameter is Window window)
		{
			window.Close();
		}
		ButtonCommandResult = CommandResult.RestartLater;
	}

	public new static LanguageSelectViewModel DefaultValues()
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		LanguageSelectViewModel languageSelectViewModel = new LanguageSelectViewModel();
		languageSelectViewModel.TitleIconImageSource = ComponentResources.SingleInstance.GetResource("drawingImage_warning") as ImageSource;
		languageSelectViewModel.LanguageSelected = LMSAContext.CurrentLanguage;
		languageSelectViewModel.TitleFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("12pt"));
		languageSelectViewModel.TitleForeground = new SolidColorBrush(Color.FromRgb(119, 119, 118));
		languageSelectViewModel.ContentFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("9pt"));
		languageSelectViewModel.ContentForeground = new SolidColorBrush(Color.FromRgb(119, 119, 118));
		languageSelectViewModel.ReadyLangTipContentFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("9pt"));
		languageSelectViewModel.ReadyLangTipContentForeground = new SolidColorBrush(Colors.Red);
		languageSelectViewModel.CancelButtonBackground = new SolidColorBrush(Color.FromRgb(229, 229, 229));
		languageSelectViewModel.CancelButtonForeground = new SolidColorBrush(Color.FromRgb(143, 143, 143));
		languageSelectViewModel.CancelButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"));
		languageSelectViewModel.RestartLaterButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		languageSelectViewModel.RestartLaterButtonForeground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		languageSelectViewModel.RestartLaterButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"));
		languageSelectViewModel.RestartNowButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		languageSelectViewModel.RestartNowButtonForeground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		languageSelectViewModel.RestartNowButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"));
		languageSelectViewModel.OKButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
		languageSelectViewModel.OKButtonForeground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		languageSelectViewModel.OKButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"));
		return languageSelectViewModel;
	}
}
