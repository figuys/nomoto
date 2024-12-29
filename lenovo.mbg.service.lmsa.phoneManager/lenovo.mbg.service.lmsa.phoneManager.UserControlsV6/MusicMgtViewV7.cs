using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class MusicMgtViewV7 : UserControl, IComponentConnector, IStyleConnector
{
	private MusicMgtViewModelV7 GetCurrentContext
	{
		get
		{
			MusicMgtViewModelV7 _dataContext = null;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				_dataContext = base.DataContext as MusicMgtViewModelV7;
			});
			return _dataContext;
		}
	}

	public MusicMgtViewV7()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			ddlStorageSelect.Cbx.SelectionChanged -= ddlStorageSelect_SelectionChanged;
			ddlStorageSelect.Cbx.SelectionChanged += ddlStorageSelect_SelectionChanged;
			if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto")
			{
				ddlStorageSelect.Visibility = Visibility.Collapsed;
			}
			else
			{
				ddlStorageSelect.Visibility = Visibility.Visible;
			}
		};
	}

	private void CheckAll_Click(object sender, RoutedEventArgs e)
	{
		bool isSelected = (e.OriginalSource as CheckBox).IsChecked ?? true;
		IEnumerable<MusicInfo> enumerable = MusicListData.ItemsSource.OfType<MusicInfo>();
		if (enumerable == null || enumerable.Count() <= 0)
		{
			return;
		}
		foreach (MusicInfo item in enumerable)
		{
			item.IsSelected = isSelected;
		}
	}

	private void SongList_DBClick(object sender, MouseButtonEventArgs e)
	{
		DataGrid dataGrid = sender as DataGrid;
		Point position = e.GetPosition(dataGrid);
		DependencyObject dependencyObject = dataGrid.InputHitTest(position) as DependencyObject;
		bool flag = false;
		while (dependencyObject != null)
		{
			if (dependencyObject is DataGridRow)
			{
				flag = true;
				break;
			}
			dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
		}
		e.Handled = !flag;
		if (flag)
		{
			MusicInfo music = MusicListData.SelectedItem as MusicInfo;
			MusicPlayerViewModelV7.SingleInstance.SongDBClick(new MusicInfoViewModelV7(music));
		}
	}

	private void SetMusicNotify(object sender, RoutedEventArgs e)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		string type = (sender as MenuItem).Tag.ToString();
		MusicInfo _item = MusicListData.SelectedItem as MusicInfo;
		Stopwatch sw = new Stopwatch();
		sw.Start();
		BusinessData businessData = new BusinessData(BusinessType.SONG_SET_RINGTONE, Context.CurrentDevice);
		HostProxy.PermissionService.BeginConfirmAppIsReady(HostProxy.deviceManager.MasterDevice, "SetRingTone", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				int code = 1;
				AsyncDataLoader.BeginLoading(delegate
				{
					string[] names = Enum.GetNames(typeof(MusicType));
					foreach (string value in names)
					{
						if (type.Equals(value))
						{
							MusicType type2 = (MusicType)Enum.Parse(typeof(MusicType), value);
							code = GetCurrentContext.m_bll.SetMusicAsRingtone(_item, type2);
							break;
						}
					}
					sw.Stop();
					if (code == 0)
					{
						HostProxy.BehaviorService.Collect(BusinessType.SONG_SET_RINGTONE, businessData.Update(sw.ElapsedMilliseconds, (code == 0) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
					}
					if (code == 0)
					{
						return new Tuple<bool, string, string>(item1: true, "K0711", "K0763");
					}
					if (code == 2)
					{
						return (Tuple<bool, string, string>)null;
					}
					if (currentDevice != null && currentDevice.Property != null && currentDevice.Property.AndroidVersion.Contains("6.0"))
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							RegrantAppPermissionTips win1 = new RegrantAppPermissionTips();
							HostProxy.HostMaskLayerWrapper.New(win1, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
							{
								win1.ShowDialog();
							});
						});
						return (Tuple<bool, string, string>)null;
					}
					return new Tuple<bool, string, string>(item1: true, "K0071", "K0553");
				}, ViewContext.SingleInstance.MainViewModel);
			}
		});
	}

	private void ddlStorageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (GetCurrentContext?.MusicToolImportBtn == null)
		{
			return;
		}
		if (GetCurrentContext.IsSelectedInternal)
		{
			GetCurrentContext.MusicToolImportBtn.IsEnabled = true;
		}
		else
		{
			GetCurrentContext.MusicToolImportBtn.IsEnabled = false;
		}
		GetCurrentContext.MusicToolRefreshCommandHandler(null);
		if (GetCurrentContext.Albums.Count > 0)
		{
			MusicAlbumViewModelV7 musicAlbumViewModelV = GetCurrentContext.Albums.First();
			musicAlbumViewModelV.IsSelected = true;
			musicAlbumViewModelV.SingleClickCommand.Execute(true);
		}
		foreach (MusicAlbumViewModelV7 album in GetCurrentContext.Albums)
		{
			album.IsSelectedAllMusics = false;
		}
	}

	private void OnCheckBoxSelectAllAlbums(object sender, RoutedEventArgs e)
	{
		bool isSelected = (sender as CheckBox).IsChecked == true;
		SelectAllAblumsMusics(isSelected);
	}

	private void SelectAllItemClick(object sender, RoutedEventArgs e)
	{
		SelectAllAblumsMusics(_isSelected: true);
	}

	private void UnselectAllItemClick(object sender, RoutedEventArgs e)
	{
		SelectAllAblumsMusics(_isSelected: false);
	}

	private void SelectAllAblumsMusics(bool _isSelected)
	{
		foreach (MusicAlbumViewModelV7 album in GetCurrentContext.Albums)
		{
			album.IsSelectedAllMusics = _isSelected;
		}
		GetCurrentContext.RefreshAllAlbumMusicSelectedCount();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
