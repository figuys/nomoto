using System.Windows;
using System.Windows.Media;
using GoogleAnalytics;
using lenovo.mbg.service.lmsa.ViewV6;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class LanguageOperationItemViewModelV6 : MouseOverMenuItemViewModel
{
	public LanguageOperationItemViewModelV6()
	{
		base.Icon = Application.Current.FindResource("option_defaultDrawingImage") as ImageSource;
		base.MouseOverIcon = Application.Current.FindResource("option_clickDrawingImage") as ImageSource;
		base.Header = "K0281";
	}

	public override void ClickCommandHandler(object e)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuOptionButtonClick", "menu-option button click", 0L).Build());
		ApplcationClass.ApplcationStartWindow.ShowMessage(new LanguageSelectViewV6());
	}
}
