using GoogleAnalytics;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class FeedbackOperationItemViewModel : MouseOverMenuItemViewModel
{
	public override void ClickCommandHandler(object e)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuFeedbackButtonClick", "menu-feedback button click", 0L).Build());
		HostProxy.HostOperationService.ShowFeedBack();
	}
}
