using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.ViewV6;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.Login.ViewModel.UserOperation;

public class DeviceListMenuItemViewModel : MouseOverMenuItemViewModel
{
	public DeviceListMenuItemViewModel()
	{
		base.Icon = Application.Current.FindResource("fileDrawingImage") as DrawingImage;
		base.MouseOverIcon = Application.Current.FindResource("fileDrawingImage_mouseover") as DrawingImage;
		base.Header = "K0705";
	}

	public override void ClickCommandHandler(object e)
	{
		if (!PermissionService.Single.CheckPermission(UserService.Single.CurrentLoggedInUser.UserId, "10", "1"))
		{
			RegisterDevView registerDevView = new RegisterDevView("K0705", IsOnlySel: false);
			registerDevView.IsExistRegistedDev();
			ApplcationClass.ApplcationStartWindow.ShowMessage(registerDevView);
		}
	}
}
