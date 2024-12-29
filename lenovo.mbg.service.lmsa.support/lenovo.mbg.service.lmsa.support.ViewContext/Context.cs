using System;
using System.Collections.Generic;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.forum;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.messenger;
using lenovo.mbg.service.lmsa.support.ViewModel;
using lenovo.mbg.service.lmsa.tips;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support.ViewContext;

public class Context
{
	public static SortedList<ViewType, ViewDescription> view = new SortedList<ViewType, ViewDescription>
	{
		{
			ViewType.WARRTETY,
			new ViewDescription(typeof(SupportFrame), typeof(SupportFrameViewModel))
		},
		{
			ViewType.TIPS,
			new ViewDescription(typeof(TipsFrame), typeof(TipsFrameViewModel))
		},
		{
			ViewType.FORUM,
			new ViewDescription(typeof(ForumFrame), typeof(ForumFrameViewModel))
		},
		{
			ViewType.MOLI,
			new ViewDescription(typeof(MessengerFrame), typeof(ViewModelBase))
		}
	};

	protected static Dictionary<ViewType, string> ViewCategoryMap = new Dictionary<ViewType, string>
	{
		{
			ViewType.TIPS,
			null
		},
		{
			ViewType.FORUM,
			null
		}
	};

	public static MainFrameViewModel mainViewModel;

	public static void ChangeCategory(ViewType viewType, string category)
	{
		if (ViewCategoryMap.ContainsKey(viewType))
		{
			ViewCategoryMap[viewType] = category;
		}
	}

	public static void Switch(ViewType viewType, string category)
	{
		bool reloadData = false;
		if (ViewCategoryMap.ContainsKey(viewType))
		{
			if (ViewCategoryMap[viewType] != category)
			{
				reloadData = true;
			}
			ViewCategoryMap[viewType] = category;
		}
		object currentView = HostProxy.ViewContext.SwitchView(view[viewType], category, reload: false, reloadData);
		mainViewModel.CurrentView = currentView;
	}

	public static void Switch(ViewType viewType, object initilizeData, bool reload)
	{
		object currentView = HostProxy.ViewContext.SwitchView(view[viewType], initilizeData, reload);
		mainViewModel.CurrentView = currentView;
	}

	public static void Switch(ViewType viewType)
	{
		object currentView = HostProxy.ViewContext.SwitchView(view[viewType]);
		mainViewModel.CurrentView = currentView;
	}

	public static FrameworkElement FindView(Type viewType)
	{
		return HostProxy.ViewContext.FindView(viewType);
	}
}
