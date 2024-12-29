using System;
using System.Windows;

namespace lenovo.mbg.service.framework.services;

public interface IViewContext
{
	FrameworkElement SwitchView(ViewDescription view);

	FrameworkElement SwitchView(ViewDescription view, object initilizeData);

	FrameworkElement SwitchView(ViewDescription view, object initilizeData, bool reload, bool reloadData = false);

	TViewModel FindViewModel<TViewModel>(Type viewType, string uiid = null) where TViewModel : IViewModelBase;

	FrameworkElement FindView(Type viewType, string uiid = null);
}
