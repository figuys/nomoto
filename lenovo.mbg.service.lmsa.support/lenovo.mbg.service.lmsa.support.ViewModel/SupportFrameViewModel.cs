using lenovo.mbg.service.lmsa.support.Commons;
using lenovo.mbg.service.lmsa.support.UserControls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class SupportFrameViewModel : ViewModelBase
{
	public object _currentView;

	public object CurrentView
	{
		get
		{
			return _currentView;
		}
		set
		{
			_currentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public SupportFrameViewModel()
	{
		lenovo.mbg.service.lmsa.support.Commons.ViewContext.FrameViewModel = this;
		InitializeView();
	}

	protected void InitializeView()
	{
		lenovo.mbg.service.lmsa.support.Commons.ViewContext.SwitchView<SearchViewEx>();
	}
}
