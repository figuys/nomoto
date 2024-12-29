using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class OperatorButtonViewModelV6 : ViewModelBase
{
	private Visibility _isVisibility;

	private volatile bool _isEnabled = true;

	public string ButtonText { get; set; }

	public string ButtonTextDisplay { get; set; }

	public Visibility Visibility
	{
		get
		{
			return _isVisibility;
		}
		set
		{
			if (_isVisibility != value)
			{
				_isVisibility = value;
				OnPropertyChanged("Visibility");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public ImageSource ButtonImageSource { get; set; }

	public ImageSource ButtonSelectedImageSource { get; set; }

	public ImageSource ButtonDisabledImageSource { get; set; }

	public ReplayCommand ClickCommand { get; set; }
}
