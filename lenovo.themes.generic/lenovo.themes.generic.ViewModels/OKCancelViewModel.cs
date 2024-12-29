using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class OKCancelViewModel : OKViewModel
{
	private bool _IsOKResult = true;

	private string _cancelButtonText = string.Empty;

	private double _cancelButtonTextFontSize = 6.0;

	private Brush _cancelButtonBackground;

	private Brush _cancelButtonForeground;

	private Brush _cancelButtonMouseOverBackground;

	private Brush _cancelButtonDisabledBackground;

	public bool IsOKResult
	{
		get
		{
			return _IsOKResult;
		}
		private set
		{
			_IsOKResult = value;
		}
	}

	public string CancelButtonText
	{
		get
		{
			return _cancelButtonText;
		}
		set
		{
			_cancelButtonText = value;
			OnPropertyChanged("CancelButtonText");
		}
	}

	public double CancelButtonTextFontSize
	{
		get
		{
			return _cancelButtonTextFontSize;
		}
		set
		{
			_cancelButtonTextFontSize = value;
			OnPropertyChanged("CancelButtonTextFontSize");
		}
	}

	public Brush CancelButtonBackground
	{
		get
		{
			return _cancelButtonBackground;
		}
		set
		{
			_cancelButtonBackground = value;
			OnPropertyChanged("CancelButtonBackground");
		}
	}

	public Brush CancelButtonForeground
	{
		get
		{
			return _cancelButtonForeground;
		}
		set
		{
			_cancelButtonForeground = value;
			OnPropertyChanged("CancelButtonForeground");
		}
	}

	public Brush CancelButtonMouseOverBackground
	{
		get
		{
			return _cancelButtonMouseOverBackground;
		}
		set
		{
			if (_cancelButtonMouseOverBackground != value)
			{
				_cancelButtonMouseOverBackground = value;
				OnPropertyChanged("CancelButtonMouseOverBackground");
			}
		}
	}

	public Brush CancelButtonDisabledBackground
	{
		get
		{
			return _cancelButtonDisabledBackground;
		}
		set
		{
			if (_cancelButtonDisabledBackground != value)
			{
				_cancelButtonDisabledBackground = value;
				OnPropertyChanged("CancelButtonDisabledBackground");
			}
		}
	}

	public ReplayCommand CancelButtonClickCommand { get; set; }

	public ReplayCommand PopCancelClick { get; }

	public ReplayCommand PopOkClick { get; }

	public bool ClosePopup { get; set; }

	protected virtual void CancelButtonClickCommandHandler(object parameter)
	{
		Window window = parameter as Window;
		if (ClosePopup)
		{
			UserControl userControl = window.Content as UserControl;
			if (userControl.Template.FindName("pop", userControl) is PopupEx { IsOpen: false } popupEx)
			{
				popupEx.IsOpen = true;
			}
		}
		else
		{
			IsOKResult = false;
			window?.Close();
		}
	}

	private void PopCancelClickHandler(object args)
	{
		Window obj = args as Window;
		UserControl userControl = obj.Content as UserControl;
		if (userControl.Template.FindName("pop", userControl) is PopupEx popupEx)
		{
			popupEx.IsOpen = false;
		}
		IsOKResult = false;
		obj?.Close();
	}

	private void PopOkClickHandler(object args)
	{
		UserControl userControl = (args as Window).Content as UserControl;
		if (userControl.Template.FindName("pop", userControl) is PopupEx { IsOpen: not false } popupEx)
		{
			popupEx.IsOpen = false;
		}
	}

	public OKCancelViewModel()
	{
		CancelButtonClickCommand = new ReplayCommand(CancelButtonClickCommandHandler);
		PopCancelClick = new ReplayCommand(PopCancelClickHandler);
		PopOkClick = new ReplayCommand(PopOkClickHandler);
	}

	protected override void OKButtonClickCommandHandler(object parameter)
	{
		base.OKButtonClickCommandHandler(parameter);
		Window window = parameter as Window;
		IsOKResult = true;
		window?.Close();
	}

	public new static OKCancelViewModel DefaultValues()
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		return new OKCancelViewModel
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
