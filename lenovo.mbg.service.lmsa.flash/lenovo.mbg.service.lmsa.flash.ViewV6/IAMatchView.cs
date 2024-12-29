using System.Windows;
using lenovo.mbg.service.lmsa.flash.Common;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public interface IAMatchView
{
	AutoMatchViewModel VM { get; }

	FrameworkElement ParentView { get; }

	FrameworkElement RescueView { get; set; }

	void Init(AutoMatchResource data, object wModel, FrameworkElement parentView);
}
