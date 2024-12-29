using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class LeftNavButtonViewModel : ViewModelBase
{
	private int _totalCount;

	private bool _IsEnabled = true;

	public int TotalCount
	{
		get
		{
			return _totalCount;
		}
		set
		{
			_totalCount = value;
			OnPropertyChanged("TotalCount");
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _IsEnabled;
		}
		set
		{
			_IsEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public string ButtonText { get; set; }

	public string ButtonTextDisplay { get; set; }

	public ImageSource ButtonImageSource { get; set; }

	public ImageSource ButtonForegroundImageSource { get; set; }

	public ImageSource ButtonSelectedImageSource { get; set; }

	public ReplayCommand ClickCommand { get; set; }
}
