using lenovo.mbg.service.common.utilities;
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
}
