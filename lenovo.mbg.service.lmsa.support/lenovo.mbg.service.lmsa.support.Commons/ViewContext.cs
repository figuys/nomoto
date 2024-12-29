using System;
using System.Collections.Generic;
using System.Windows;
using lenovo.mbg.service.lmsa.support.ViewModel;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support.Commons;

public class ViewContext
{
	protected static SortedList<string, FrameworkElement> _Cache = new SortedList<string, FrameworkElement>();

	public static SupportFrameViewModel FrameViewModel { get; set; }

	public static FrameworkElement SwitchView<ViewType>()
	{
		return SwitchView<ViewType>(reload: false);
	}

	public static FrameworkElement SwitchView<ViewType>(bool reload)
	{
		Type typeFromHandle = typeof(ViewType);
		string fullName = typeFromHandle.FullName;
		FrameworkElement frameworkElement = null;
		if (_Cache.ContainsKey(fullName))
		{
			frameworkElement = _Cache[fullName];
		}
		if (reload || frameworkElement == null)
		{
			frameworkElement = Activator.CreateInstance(typeFromHandle) as FrameworkElement;
			(frameworkElement.DataContext as ViewModelBase)?.LoadData();
			_Cache[fullName] = frameworkElement;
		}
		if (FrameViewModel != null)
		{
			FrameViewModel.CurrentView = frameworkElement;
		}
		return frameworkElement;
	}

	public static ViewModelType GetViewMoedel<ViewModelType>(Type viewType) where ViewModelType : ViewModelBase
	{
		FrameworkElement frameworkElement = _Cache[viewType.FullName];
		if (frameworkElement == null)
		{
			return null;
		}
		return (ViewModelType)frameworkElement.DataContext;
	}
}
