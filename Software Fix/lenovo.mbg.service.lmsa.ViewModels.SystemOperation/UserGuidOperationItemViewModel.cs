using System.Windows;
using System.Windows.Media;
using GoogleAnalytics;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Business;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class UserGuidOperationItemViewModel : MouseOverMenuItemViewModel
{
	public UserGuidOperationItemViewModel()
	{
		base.Icon = Application.Current.FindResource("guidanceDrawingImage") as ImageSource;
		base.MouseOverIcon = Application.Current.FindResource("guidance_clickDrawingImage") as ImageSource;
		base.Header = "K0279";
	}

	public override void ClickCommandHandler(object e)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuUserGuideButtonClick", "menu-user guide button click", 0L).Build());
		GlobalFun.OpenUrlByBrowser(new MenuPopupWindowBusiness().GetUserGuideUrl());
	}
}
