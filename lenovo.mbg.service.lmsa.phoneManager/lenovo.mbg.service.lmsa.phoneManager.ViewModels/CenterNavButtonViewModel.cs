using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class CenterNavButtonViewModel : NotifyBase
{
	private Visibility _countVisiblity = Visibility.Hidden;

	private int _count;

	public Visibility CountVisiblity
	{
		get
		{
			return _countVisiblity;
		}
		set
		{
			if (_countVisiblity != value)
			{
				_countVisiblity = value;
				OnPropertyChanged("TipsVisiblity");
			}
		}
	}

	public int Count
	{
		get
		{
			return _count;
		}
		set
		{
			if (_count != value)
			{
				_count = value;
				CountVisiblity = ((_count <= 0) ? Visibility.Hidden : Visibility.Visible);
				OnPropertyChanged("Count");
			}
		}
	}

	public Thickness ItemContainerMargin { get; set; }

	public string ButtonText { get; set; }

	public string ButtonTextDisplay { get; set; }

	public ImageSource ButtonImageSource { get; set; }

	public ReplayCommand ClickCommand { get; set; }

	public CenterNavButtonViewModel()
	{
		ClickCommand = new ReplayCommand(ClickHandler);
	}

	protected void ClickHandler(object parameter)
	{
	}
}
