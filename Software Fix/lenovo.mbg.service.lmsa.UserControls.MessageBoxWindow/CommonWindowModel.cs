using System.Windows.Controls;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.UserControls.MessageBoxWindow;

public class CommonWindowModel : OkCancelWindowModel
{
	public CommonWindowModel(int type)
	{
		if (type == 2)
		{
			base.ContentControlTemplate = ComponentResources.SingleInstance.GetResource("defaultOKCancelWindowTemplate") as ControlTemplate;
		}
		else
		{
			base.ContentControlTemplate = ComponentResources.SingleInstance.GetResource("defaultOKWindowTemplate") as ControlTemplate;
		}
	}

	protected override lenovo.themes.generic.ViewModelV6.ViewModelBase GetDefaultViewModel()
	{
		return OKCancelViewModel.DefaultValues();
	}
}
