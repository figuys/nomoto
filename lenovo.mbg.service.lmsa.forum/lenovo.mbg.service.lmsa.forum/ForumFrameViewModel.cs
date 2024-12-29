using System.Windows;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.forum;

public class ForumFrameViewModel : ViewModelBase
{
	private ForumFrame view;

	public ForumFrameViewModel()
	{
		FrameworkElement frameworkElement = HostProxy.ViewContext.FindView(typeof(ForumFrame));
		if (frameworkElement != null)
		{
			view = frameworkElement as ForumFrame;
		}
	}

	public override void LoadData(object data)
	{
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			view?.SwitchChileView();
		});
		view?.GoToTargetUrl(data?.ToString());
		base.LoadData(data);
	}
}
