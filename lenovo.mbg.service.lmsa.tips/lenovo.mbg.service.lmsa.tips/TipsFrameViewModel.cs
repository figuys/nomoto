using System.Windows;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.tips;

public class TipsFrameViewModel : ViewModelBase
{
	private TipsFrame view;

	public TipsFrameViewModel()
	{
		FrameworkElement frameworkElement = HostProxy.ViewContext.FindView(typeof(TipsFrame));
		if (frameworkElement != null)
		{
			view = frameworkElement as TipsFrame;
		}
	}

	public override void LoadData(object data)
	{
		view?.GoToUrl(data?.ToString());
		base.LoadData(data);
	}
}
