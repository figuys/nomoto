using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class AppMgtViewV6 : UserControl, IComponentConnector, IStyleConnector
{
	private AppMgtViewModelV6 GetCurrentContext
	{
		get
		{
			AppMgtViewModelV6 _dataContext = null;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				_dataContext = base.DataContext as AppMgtViewModelV6;
			});
			return _dataContext;
		}
	}

	public AppMgtViewV6()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			AppMgtViewModelV6 getCurrentContext = GetCurrentContext;
			getCurrentContext.RefreshFinishHandler += delegate
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					AppsViewChange(null, null);
				});
			};
			getCurrentContext.MyAppList.CollectionChanged += MyAppList_CollectionChanged;
			getCurrentContext.SysAppList.CollectionChanged += SysAppList_CollectionChanged;
			getCurrentContext.AppType = AppType.MyApp;
			TopCategory.SelectedIndex = 0;
		};
	}

	private void MyAppList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (GetCurrentContext.MyAppList.Any((AppInfoModel a) => a.IsSelected))
			{
				btnExport.IsEnabled = true;
				SetUninstallButtonStatus();
			}
			else
			{
				btnExport.IsEnabled = false;
				SetUninstallButtonStatus();
			}
		});
	}

	private void SysAppList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (GetCurrentContext.SysAppList.Any((AppInfoModel a) => a.IsSelected))
			{
				btnExport.IsEnabled = true;
				SetUninstallButtonStatus();
			}
			else
			{
				btnExport.IsEnabled = false;
				SetUninstallButtonStatus();
			}
		});
	}

	private void AppsViewChange(object sender, SelectionChangedEventArgs e)
	{
		AppMgtViewModelV6 getCurrentContext = GetCurrentContext;
		if (TopCategory.SelectedIndex == 0)
		{
			AppListData.ItemsSource = getCurrentContext.MyAppList;
			getCurrentContext.AppType = AppType.MyApp;
			AppListData.Columns[2].Visibility = Visibility.Visible;
			AppListData.Columns[3].Visibility = Visibility.Visible;
			AppListData.Columns[5].Visibility = Visibility.Visible;
		}
		else if (TopCategory.SelectedIndex == 1)
		{
			AppListData.ItemsSource = getCurrentContext.SysAppList;
			getCurrentContext.AppType = AppType.SystemApp;
			AppListData.Columns[5].Visibility = Visibility.Collapsed;
			AppListData.Columns[2].Visibility = Visibility.Collapsed;
			AppListData.Columns[3].Visibility = Visibility.Collapsed;
		}
		ObservableCollection<AppInfoModel> observableCollection = AppListData.ItemsSource as ObservableCollection<AppInfoModel>;
		if (observableCollection.Count == 0 || observableCollection.Any((AppInfoModel a) => !a.IsSelected))
		{
			getCurrentContext.IsAllSelected = false;
		}
		else
		{
			getCurrentContext.IsAllSelected = true;
		}
		SetButtonState();
	}

	private void CheckAllClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is CheckBox { IsChecked: not null } checkBox))
		{
			return;
		}
		ObservableCollection<AppInfoModel> observableCollection = AppListData.ItemsSource as ObservableCollection<AppInfoModel>;
		if (observableCollection != null)
		{
			foreach (AppInfoModel item in observableCollection)
			{
				item.IsSelected = checkBox.IsChecked.Value;
			}
		}
		if (checkBox.IsChecked == true && observableCollection.Count != 0)
		{
			btnExport.IsEnabled = true;
			SetUninstallButtonStatus();
			GetCurrentContext.AppChooseCount = observableCollection.Count;
		}
		else
		{
			btnExport.IsEnabled = false;
			SetUninstallButtonStatus();
			GetCurrentContext.AppChooseCount = 0;
		}
	}

	private void CellCheckBoxClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is CheckBox { IsChecked: not null } checkBox))
		{
			return;
		}
		AppMgtViewModelV6 getCurrentContext = GetCurrentContext;
		if (checkBox.IsChecked == true)
		{
			getCurrentContext.AppChooseCount++;
			ObservableCollection<AppInfoModel> source = ((TopCategory.SelectedIndex == 1) ? getCurrentContext.SysAppList : getCurrentContext.MyAppList);
			getCurrentContext.IsAllSelected = source.FirstOrDefault((AppInfoModel p) => !p.IsSelected) == null;
		}
		else
		{
			getCurrentContext.AppChooseCount--;
			getCurrentContext.IsAllSelected = false;
		}
		SetButtonState();
	}

	private void SetButtonState()
	{
		if ((AppListData.ItemsSource as ObservableCollection<AppInfoModel>).Any((AppInfoModel a) => a.IsSelected))
		{
			btnExport.IsEnabled = true;
			SetUninstallButtonStatus();
		}
		else
		{
			btnExport.IsEnabled = false;
			SetUninstallButtonStatus();
		}
	}

	private void btnUninstallClick(object sender, RoutedEventArgs e)
	{
		AppInfoModel appinfo = (sender as Button).CommandParameter as AppInfoModel;
		GetCurrentContext.UninstallAppSingle(appinfo);
	}

	private void Sort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		AppListData.Columns[0].SortDirection = ((AppListData.Columns[1].SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending);
	}

	private void SetUninstallButtonStatus()
	{
		btnUninstall.IsEnabled = btnExport.IsEnabled;
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice != null && masterDevice.ConnectType == ConnectType.Wifi)
		{
			btnUninstall.IsEnabled = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
