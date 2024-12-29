using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ProgressLoadingWindowViewModel : ViewModelBase
{
	private string _Message = string.Empty;

	public string Message
	{
		get
		{
			return _Message;
		}
		set
		{
			_Message = value;
			OnPropertyChanged("Message");
		}
	}

	public ProgressLoadingWindowViewModel()
	{
		_Message = "K0671";
	}
}
