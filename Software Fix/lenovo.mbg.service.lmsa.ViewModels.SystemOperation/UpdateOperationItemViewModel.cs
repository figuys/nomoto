using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GoogleAnalytics;
using lenovo.mbg.service.lmsa.UpdateVersion;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class UpdateOperationItemViewModel : MouseOverMenuItemViewModel
{
	public UpdateOperationItemViewModel()
	{
		base.Icon = Application.Current.FindResource("updateDrawingImage") as ImageSource;
		base.MouseOverIcon = Application.Current.FindResource("update_clickDrawingImage") as ImageSource;
		base.Header = "K0280";
	}

	public override void ClickCommandHandler(object e)
	{
		ApplcationClass.manualTrigger = true;
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuCheckUpdateButtonClick", "menu-check update button click", 0L).Build());
		Task.Factory.StartNew(delegate
		{
			UpdateManager.Instance.ToolUpdateWorker.CheckVersion(isAutoMode: false);
		});
	}
}
