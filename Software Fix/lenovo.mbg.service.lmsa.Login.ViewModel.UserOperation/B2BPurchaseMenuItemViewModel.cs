using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.Login.ViewModel.UserOperation;

public class B2BPurchaseMenuItemViewModel : MouseOverMenuItemViewModel
{
	public B2BPurchaseMenuItemViewModel()
	{
		base.Header = "K1688";
	}

	public override void ClickCommandHandler(object e)
	{
		ApplcationClass.ApplcationStartWindow.ShowB2BPurchaseOverview();
	}
}
