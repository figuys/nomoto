using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.LenovoId;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.Login.ViewModel.UserOperation;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Login.ViewModel;

public class UserOperationMenuViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private Dictionary<MouseOverMenuItemViewModel, bool> itemsActiveStatus = new Dictionary<MouseOverMenuItemViewModel, bool>();

	private ObservableCollection<MouseOverMenuItemViewModel> m_menumItemsSource;

	private ReplayCommand mLogIn;

	private bool mIsOnline = false;

	private string mUserName;

	private Visibility mLoginButtonVisibility = Visibility.Visible;

	private Visibility mUserOperationMenuVisibility = Visibility.Collapsed;

	private string mLoginButtonText = "K0006";

	public ObservableCollection<MouseOverMenuItemViewModel> MenuItems
	{
		get
		{
			return m_menumItemsSource;
		}
		set
		{
			if (m_menumItemsSource != value)
			{
				m_menumItemsSource = value;
				OnPropertyChanged("MenuItems");
			}
		}
	}

	public ReplayCommand LogInCommand
	{
		get
		{
			return mLogIn;
		}
		set
		{
			if (mLogIn != value)
			{
				mLogIn = value;
				OnPropertyChanged("LogInCommand");
			}
		}
	}

	public bool IsOnline
	{
		get
		{
			return mIsOnline;
		}
		set
		{
			if (mIsOnline != value)
			{
				mIsOnline = value;
				OnPropertyChanged("IsOnline");
			}
		}
	}

	public string UserName
	{
		get
		{
			return mUserName;
		}
		set
		{
			if (!(mUserName == value))
			{
				mUserName = value;
				OnPropertyChanged("UserName");
			}
		}
	}

	public Visibility LoginButtonVisibility
	{
		get
		{
			return mLoginButtonVisibility;
		}
		set
		{
			if (mLoginButtonVisibility != value)
			{
				mLoginButtonVisibility = value;
				OnPropertyChanged("LoginButtonVisibility");
			}
		}
	}

	public Visibility UserOperationMenuVisibility
	{
		get
		{
			return mUserOperationMenuVisibility;
		}
		set
		{
			if (mUserOperationMenuVisibility != value)
			{
				mUserOperationMenuVisibility = value;
				OnPropertyChanged("UserOperationMenuVisibility");
			}
		}
	}

	public string LoginButtonText
	{
		get
		{
			return mLoginButtonText;
		}
		set
		{
			if (!(mLoginButtonText == value))
			{
				mLoginButtonText = value;
				OnPropertyChanged("LoginButtonText");
			}
		}
	}

	public UserOperationMenuViewModel()
	{
		MenuItems = new ObservableCollection<MouseOverMenuItemViewModel>();
		DeviceListMenuItemViewModel deviceListMenuItemViewModel = new DeviceListMenuItemViewModel
		{
			ItemVisibility = Visibility.Collapsed
		};
		itemsActiveStatus.Add(deviceListMenuItemViewModel, value: true);
		MenuItems.Add(deviceListMenuItemViewModel);
		B2BPurchaseMenuItemViewModel b2BPurchaseMenuItemViewModel = new B2BPurchaseMenuItemViewModel
		{
			ItemVisibility = Visibility.Collapsed
		};
		itemsActiveStatus.Add(b2BPurchaseMenuItemViewModel, value: true);
		MenuItems.Add(b2BPurchaseMenuItemViewModel);
		LogoutMenuItemViewModel logoutMenuItemViewModel = new LogoutMenuItemViewModel
		{
			ItemVisibility = Visibility.Collapsed
		};
		MenuItems.Add(logoutMenuItemViewModel);
		itemsActiveStatus.Add(logoutMenuItemViewModel, value: true);
		LogInCommand = new ReplayCommand(LogInCommandHandler);
	}

	public void ActiveItem(Type type)
	{
		if (type == null)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			KeyValuePair<MouseOverMenuItemViewModel, bool> keyValuePair = itemsActiveStatus.Where((KeyValuePair<MouseOverMenuItemViewModel, bool> m) => m.Key.GetType().Equals(type)).FirstOrDefault();
			if (itemsActiveStatus.ContainsKey(keyValuePair.Key))
			{
				keyValuePair.Key.ItemVisibility = Visibility.Collapsed;
				itemsActiveStatus[keyValuePair.Key] = false;
			}
		});
	}

	public void OnlineUserChangedHandler(object sender, OnlineUserChangedEventArgs e)
	{
		if (IsOnline = e.IsOnline)
		{
			LoginButtonVisibility = Visibility.Collapsed;
			UserOperationMenuVisibility = Visibility.Visible;
			if ("lmsa".Equals(e.UserInfo.UserSource))
			{
				UserName = e.UserInfo.UserName;
			}
			else
			{
				UserName = e.UserInfo.FullName;
			}
			bool flag = PermissionService.Single.CheckPermission(UserService.Single.CurrentLoggedInUser.UserId, "10", "1");
			{
				foreach (MouseOverMenuItemViewModel menuItem in MenuItems)
				{
					if (flag && menuItem is DeviceListMenuItemViewModel)
					{
						if (itemsActiveStatus[menuItem])
						{
							menuItem.ItemVisibility = Visibility.Collapsed;
						}
					}
					else if (menuItem is B2BPurchaseMenuItemViewModel)
					{
						menuItem.ItemVisibility = ((!UserService.Single.CurrentLoggedInUser.B2BEntranceEnable) ? Visibility.Collapsed : Visibility.Visible);
					}
					else
					{
						menuItem.ItemVisibility = Visibility.Visible;
					}
				}
				return;
			}
		}
		PermissionService.Single.Stop();
		UserName = string.Empty;
		LoginButtonVisibility = Visibility.Visible;
		UserOperationMenuVisibility = Visibility.Collapsed;
		foreach (MouseOverMenuItemViewModel menuItem2 in MenuItems)
		{
			menuItem2.ItemVisibility = Visibility.Collapsed;
		}
	}

	private void LogInCommandHandler(object e)
	{
		if (!UserService.Single.IsOnline)
		{
			LogHelper.LogInstance.Debug("click login button in the main window");
			LenovoIdWindow.ShowDialogEx();
		}
	}
}
