using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa;

public class ViewContext : IViewContext
{
	protected static SortedList<string, ViewDescription> viewCache = new SortedList<string, ViewDescription>();

	public FrameworkElement SwitchView(ViewDescription view)
	{
		return SwitchView(view, null, reload: false);
	}

	public FrameworkElement SwitchView(ViewDescription view, object initilizeData)
	{
		return SwitchView(view, initilizeData, reload: false);
	}

	public FrameworkElement SwitchView(ViewDescription view, object initilizeData, bool reload, bool reloadData = false)
	{
		string uIID = view.UIID;
		if (reload || !viewCache.ContainsKey(uIID))
		{
			if (reload && viewCache.ContainsKey(uIID))
			{
				lenovo.themes.generic.ViewModelV6.ViewModelBase viewModelBase = viewCache[uIID].ViewModel as lenovo.themes.generic.ViewModelV6.ViewModelBase;
				viewModelBase.Dispose();
			}
			view.View = (FrameworkElement)Activator.CreateInstance(view.ViewType);
			viewCache[uIID] = view;
			view.ViewModel = (IViewModelBase)Activator.CreateInstance(view.ViewModelType);
			view.View.DataContext = view.ViewModel;
		}
		ViewDescription viewDescription = viewCache[uIID];
		lenovo.themes.generic.ViewModelV6.ViewModelBase viewModelBase2 = viewDescription.ViewModel as lenovo.themes.generic.ViewModelV6.ViewModelBase;
		if (!viewModelBase2.DataIsLoaded || reloadData)
		{
			viewModelBase2.LoadData(initilizeData);
		}
		return viewDescription.View;
	}

	public TViewModel FindViewModel<TViewModel>(Type viewType, string uiid = null) where TViewModel : IViewModelBase
	{
		string key = viewType.FullName + uiid;
		if (viewCache.ContainsKey(key))
		{
			return (TViewModel)viewCache[key].ViewModel;
		}
		return default(TViewModel);
	}

	public FrameworkElement FindView(Type viewType, string uiid = null)
	{
		string key = viewType.FullName + uiid;
		if (viewCache.ContainsKey(key))
		{
			return viewCache[key].View;
		}
		return null;
	}

	public void UnActivateAsync(string viewKey)
	{
		Task.Factory.StartNew(delegate
		{
			foreach (KeyValuePair<string, ViewDescription> item in viewCache)
			{
				lenovo.themes.generic.ViewModelV6.ViewModelBase viewModelBase = item.Value.ViewModel as lenovo.themes.generic.ViewModelV6.ViewModelBase;
				if (item.Key == viewKey)
				{
					viewModelBase.Active(actived: true);
				}
				else
				{
					viewModelBase.UnActivate();
				}
			}
		});
	}

	public static void Reset()
	{
		viewCache.Clear();
	}
}
