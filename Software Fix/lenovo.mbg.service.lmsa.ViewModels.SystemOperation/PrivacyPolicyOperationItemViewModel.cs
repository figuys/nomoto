using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class PrivacyPolicyOperationItemViewModel : MouseOverMenuItemViewModel
{
	public PrivacyPolicyOperationItemViewModel()
	{
		base.Header = "K0836";
	}

	public override void ClickCommandHandler(object e)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}
}
