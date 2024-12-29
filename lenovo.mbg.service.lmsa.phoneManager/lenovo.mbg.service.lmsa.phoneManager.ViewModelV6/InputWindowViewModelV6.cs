using System;
using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class InputWindowViewModelV6 : OKCancelViewModel
{
	private string _tipText = string.Empty;

	private Brush _tipForeground;

	private double _tipFontSize = 6.0;

	public string TipText
	{
		get
		{
			return _tipText;
		}
		set
		{
			_tipText = value;
			OnPropertyChanged("TipText");
		}
	}

	public Brush TipForeground
	{
		get
		{
			return _tipForeground;
		}
		set
		{
			_tipForeground = value;
			OnPropertyChanged("TipForeground");
		}
	}

	public double TipFontSize
	{
		get
		{
			return _tipFontSize;
		}
		set
		{
			_tipFontSize = value;
			OnPropertyChanged("TipFontSize");
		}
	}

	public new static InputWindowViewModelV6 DefaultValues()
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		return new InputWindowViewModelV6
		{
			TitleIconImageSource = (ComponentResources.SingleInstance.GetResource("drawingImage_warning") as ImageSource),
			TitleFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("11pt")),
			TitleForeground = new SolidColorBrush(Color.FromRgb(143, 143, 143)),
			ContentFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("9pt")),
			ContentForeground = new SolidColorBrush(Color.FromRgb(160, 160, 160)),
			TipFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("6pt")),
			TipForeground = new SolidColorBrush(Colors.Red),
			CancelButtonBackground = new SolidColorBrush(Color.FromRgb(229, 229, 229)),
			CancelButtonForeground = new SolidColorBrush(Color.FromRgb(143, 143, 143)),
			CancelButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("11pt")),
			OKButtonBackground = new SolidColorBrush(Color.FromRgb(2, 188, 164)),
			OKButtonForeground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue)),
			OKButtonTextFontSize = Convert.ToDouble(fontSizeConverter.ConvertFromString("11pt"))
		};
	}
}
