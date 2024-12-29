using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ViewModels.SystemOperation;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModel.SystemOperation;

public class SystemOperationViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private List<MouseOverMenuItemViewModel> mRestrictedByPluginItems = new List<MouseOverMenuItemViewModel>();

	private volatile bool mIsEnabled = true;

	private Timer mTimer;

	private ObservableCollection<MouseOverMenuItemViewModel> menuItems;

	public ObservableCollection<MouseOverMenuItemViewModel> MenuItems
	{
		get
		{
			return menuItems;
		}
		set
		{
			if (menuItems != value)
			{
				menuItems = value;
				OnPropertyChanged("MenuItems");
			}
		}
	}

	public SystemOperationViewModel()
	{
		MenuItems = new ObservableCollection<MouseOverMenuItemViewModel>();
		MenuItems.Add(new PrivacyPolicyOperationItemViewModel
		{
			ItemVisibility = Visibility.Visible
		});
		UpdateOperationItemViewModel item = new UpdateOperationItemViewModel
		{
			ItemVisibility = Visibility.Visible
		};
		MenuItems.Add(item);
		LanguageOperationItemViewModelV6 item2 = new LanguageOperationItemViewModelV6
		{
			ItemVisibility = Visibility.Visible
		};
		MenuItems.Add(item2);
		MenuItems.Add(new AboutOperationItemViewModel
		{
			ItemVisibility = Visibility.Visible
		});
		mRestrictedByPluginItems.Add(item);
		mRestrictedByPluginItems.Add(item2);
		mTimer = new Timer(delegate
		{
			bool flag = MainWindowControl.Instance.IsExecuteWork();
			if (flag && mIsEnabled)
			{
				mIsEnabled = false;
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					foreach (MouseOverMenuItemViewModel mRestrictedByPluginItem in mRestrictedByPluginItems)
					{
						mRestrictedByPluginItem.IsEnabled = false;
					}
				});
			}
			else if (!flag && !mIsEnabled)
			{
				mIsEnabled = true;
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					foreach (MouseOverMenuItemViewModel mRestrictedByPluginItem2 in mRestrictedByPluginItems)
					{
						mRestrictedByPluginItem2.IsEnabled = true;
					}
				});
			}
		}, null, 1000, 1000);
	}
}
