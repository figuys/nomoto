using System;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class PrivacyPopViewModel : OKCancelViewModel
{
	private string _privacyTipsText = "K0836";

	private string _privacyUrlText = "www.lenovo.com/privacy/";

	public ReplayCommand ShowPrivacyPageClickCommand { get; set; }

	public string PrivacyTipsText
	{
		get
		{
			return _privacyTipsText;
		}
		set
		{
			_privacyTipsText = value;
			OnPropertyChanged("PrivacyTipsText");
		}
	}

	public string PrivacyUrlText
	{
		get
		{
			return _privacyUrlText;
		}
		set
		{
			_privacyUrlText = value;
			OnPropertyChanged("PrivacyUrlText");
		}
	}

	public PrivacyPopViewModel()
	{
		ShowPrivacyPageClickCommand = new ReplayCommand(ShowPrivacyPageClickCommandHandler);
	}

	private void ShowPrivacyPageClickCommandHandler(object parameter)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}

	public new static PrivacyPopViewModel DefaultValues()
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		return new PrivacyPopViewModel
		{
			TitleIconImageSource = (ComponentResources.SingleInstance.GetResource("drawingImage_warning") as ImageSource),
			TitleFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("12pt")),
			TitleForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#494949")),
			ContentFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("9pt")),
			ContentForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#777776")),
			CancelButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#efefef")),
			CancelButtonForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#898989")),
			CancelButtonMouseOverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d6d7d8")),
			CancelButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt")),
			OKButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2")),
			OKButtonMouseOverBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2")),
			OKButtonForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")),
			OKButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("10pt"))
		};
	}
}
