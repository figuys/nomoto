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
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class AppMgtView : UserControl, IComponentConnector, IStyleConnector
{
	public AppMgtView()
	{
		InitializeComponent();
		AppMgtViewModel singleInstance = AppMgtViewModel.SingleInstance;
		singleInstance.MyAppList.CollectionChanged += MyAppList_CollectionChanged;
		base.DataContext = singleInstance;
		AppListData.ItemsSource = singleInstance.MyAppList;
		TopCategory.SelectedIndex = 0;
		singleInstance.AppType = AppType.MyApp;
	}

	private void MyAppList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (AppMgtViewModel.SingleInstance.MyAppList.Any((AppInfoModel a) => a.IsSelected))
			{
				IconButton iconButton = btnExport;
				bool isEnabled = (btnUninstall.IsEnabled = true);
				iconButton.IsEnabled = isEnabled;
			}
			else
			{
				IconButton iconButton2 = btnExport;
				bool isEnabled = (btnUninstall.IsEnabled = false);
				iconButton2.IsEnabled = isEnabled;
			}
		});
	}

	private void AppsViewChange(object sender, SelectionChangedEventArgs e)
	{
		AppMgtViewModel appMgtViewModel = base.DataContext as AppMgtViewModel;
		if (TopCategory.SelectedIndex == 0)
		{
			AppListData.ItemsSource = appMgtViewModel.MyAppList;
			appMgtViewModel.AppType = AppType.MyApp;
			AppListData.Columns[2].Visibility = Visibility.Visible;
			AppListData.Columns[3].Visibility = Visibility.Visible;
			AppListData.Columns[5].Visibility = Visibility.Visible;
		}
		else if (TopCategory.SelectedIndex == 1)
		{
			AppListData.ItemsSource = appMgtViewModel.SysAppList;
			appMgtViewModel.AppType = AppType.SystemApp;
			AppListData.Columns[5].Visibility = Visibility.Collapsed;
			AppListData.Columns[2].Visibility = Visibility.Collapsed;
			AppListData.Columns[3].Visibility = Visibility.Collapsed;
		}
		if ((AppListData.ItemsSource as ObservableCollection<AppInfoModel>).Any((AppInfoModel a) => !a.IsSelected))
		{
			appMgtViewModel.IsAllSelected = false;
		}
		else
		{
			appMgtViewModel.IsAllSelected = true;
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
			btnUninstall.IsEnabled = true;
			AppMgtViewModel.SingleInstance.AppChooseCount = observableCollection.Count;
		}
		else
		{
			btnExport.IsEnabled = false;
			btnUninstall.IsEnabled = false;
			AppMgtViewModel.SingleInstance.AppChooseCount = 0;
		}
	}

	private void CellCheckBoxClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is CheckBox { IsChecked: not null } checkBox))
		{
			return;
		}
		AppMgtViewModel appMgtViewModel = base.DataContext as AppMgtViewModel;
		if (checkBox.IsChecked == true)
		{
			AppMgtViewModel.SingleInstance.AppChooseCount++;
			ObservableCollection<AppInfoModel> source = ((TopCategory.SelectedIndex == 1) ? appMgtViewModel.SysAppList : appMgtViewModel.MyAppList);
			appMgtViewModel.IsAllSelected = source.FirstOrDefault((AppInfoModel p) => !p.IsSelected) == null;
		}
		else
		{
			AppMgtViewModel.SingleInstance.AppChooseCount--;
			appMgtViewModel.IsAllSelected = false;
		}
		SetButtonState();
	}

	private void SetButtonState()
	{
		if ((AppListData.ItemsSource as ObservableCollection<AppInfoModel>).Any((AppInfoModel a) => a.IsSelected))
		{
			IconButton iconButton = btnExport;
			bool isEnabled = (btnUninstall.IsEnabled = true);
			iconButton.IsEnabled = isEnabled;
		}
		else
		{
			IconButton iconButton2 = btnExport;
			bool isEnabled = (btnUninstall.IsEnabled = false);
			iconButton2.IsEnabled = isEnabled;
		}
	}

	private void btnUninstallClick(object sender, RoutedEventArgs e)
	{
		AppInfoModel appinfo = (sender as Button).CommandParameter as AppInfoModel;
		(base.DataContext as AppMgtViewModel).UninstallAppSingle(appinfo);
	}

	private void Sort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		AppListData.Columns[0].SortDirection = ((AppListData.Columns[1].SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
