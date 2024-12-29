using System;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.ViewModels;

namespace lenovo.themes.generic.Controls;

public class PrivacyPopWindowModel : OkCancelWindowModel
{
	public PrivacyPopWindowModel()
	{
		base.ViewModel = PrivacyPopViewModel.DefaultValues();
		ControlTemplate contentControlTemplate = ComponentResources.SingleInstance.GetResource("defaultOKCancelPrivacyWindowTemplate") as ControlTemplate;
		base.ContentControlTemplate = contentControlTemplate;
	}

	public LenovoPopupWindow CreateWindow(IntPtr ownerHandler, string title, string content, string ok)
	{
		PrivacyPopViewModel obj = base.ViewModel as PrivacyPopViewModel;
		obj.Title = title;
		obj.Content = content;
		obj.CancelButtonText = "K0208";
		obj.OKButtonText = ok;
		return CreateWindow();
	}
}
