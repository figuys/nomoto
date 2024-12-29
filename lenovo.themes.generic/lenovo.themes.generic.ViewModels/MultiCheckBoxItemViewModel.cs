using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class MultiCheckBoxItemViewModel : ViewModelBase
{
	private object displayContent;

	private bool isChecked;

	public object DisplayContent
	{
		get
		{
			return displayContent;
		}
		set
		{
			if (displayContent != value)
			{
				displayContent = value;
				OnPropertyChanged("DisplayContent");
			}
		}
	}

	public bool IsChecked
	{
		get
		{
			return isChecked;
		}
		set
		{
			if (isChecked != value)
			{
				isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}

	public object Tag { get; set; }
}
