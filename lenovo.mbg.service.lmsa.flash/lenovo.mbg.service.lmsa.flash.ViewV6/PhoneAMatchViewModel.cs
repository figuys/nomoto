using System.Windows;
using lenovo.mbg.service.lmsa.flash.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class PhoneAMatchViewModel : AutoMatchViewModel
{
	public PhoneAMatchViewModel(PhoneAMatchViewV6 ui)
		: base(ui, DevCategory.Phone)
	{
		base.HelpVisibile = ((!Plugin.SupportMulti) ? Visibility.Collapsed : Visibility.Visible);
	}
}
