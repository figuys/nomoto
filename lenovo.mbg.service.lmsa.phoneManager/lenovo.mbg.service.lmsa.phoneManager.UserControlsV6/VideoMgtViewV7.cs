using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class VideoMgtViewV7 : UserControl, IComponentConnector, IStyleConnector
{
	private VideoMgtViewModelV7 GetCurrentContext
	{
		get
		{
			VideoMgtViewModelV7 _dataContext = null;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				_dataContext = base.DataContext as VideoMgtViewModelV7;
			});
			return _dataContext;
		}
	}

	public VideoMgtViewV7()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			ddlStorageSelect.Cbx.SelectionChanged -= ddlStorageSelect_SelectionChanged;
			ddlStorageSelect.Cbx.SelectionChanged += ddlStorageSelect_SelectionChanged;
			base.DataContext = GetCurrentContext;
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

	private void OnCheckBoxSelectAllInAlbum(object sender, RoutedEventArgs e)
	{
		if (sender is CheckBox { IsChecked: not null } checkBox)
		{
			bool _checked = checkBox.IsChecked.Value;
			GetCurrentContext.FocusedAlbum.CachedAllVideos.All(delegate(VideoInfoViewModelV7 v)
			{
				v.IsSelected = _checked;
				return true;
			});
			OperatorButtonViewModelV6 videoToolExportBtn = GetCurrentContext.VideoToolExportBtn;
			bool isEnabled = (GetCurrentContext.VideoToolDeleteBtn.IsEnabled = GetCurrentContext.FocusedAlbum.CachedAllVideos.Count != 0 && checkBox.IsChecked.Value);
			videoToolExportBtn.IsEnabled = isEnabled;
			if (!checkBox.IsChecked.Value && lbTileView.SelectedItems.Count > 0)
			{
				lbTileView.SelectedIndex = -1;
			}
		}
	}

	private void VideoTileList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)(e.Delta / 2));
		e.Handled = true;
	}

	private void OnCheckBoxSelectAllAlbums(object sender, RoutedEventArgs e)
	{
		bool isSelected = (sender as CheckBox).IsChecked == true;
		SelectAllAblumsVideos(isSelected);
	}

	private void SelectAllItemClick(object sender, RoutedEventArgs e)
	{
		SelectAllAblumsVideos(_isSelected: true);
	}

	private void UnselectAllItemClick(object sender, RoutedEventArgs e)
	{
		SelectAllAblumsVideos(_isSelected: false);
	}

	private void SelectAllAblumsVideos(bool _isSelected)
	{
		foreach (VideoAlbumViewModelV7 album in GetCurrentContext.AlbumList)
		{
			if (album.CachedAllVideos.Count == 0)
			{
				album.SingleClickCommand.Execute(true);
			}
			album.IsSelectedAllVideo = _isSelected;
		}
		GetCurrentContext.RefreshAllAlbumPicSelectedCount();
	}

	private void ddlStorageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (GetCurrentContext?.VideoToolImportBtn == null)
		{
			return;
		}
		if (GetCurrentContext.IsSelectedInternal)
		{
			GetCurrentContext.VideoToolImportBtn.IsEnabled = true;
		}
		else
		{
			GetCurrentContext.VideoToolImportBtn.IsEnabled = false;
		}
		GetCurrentContext.RefreshCommandHandler(null);
		if (GetCurrentContext.AlbumList.Count > 0)
		{
			VideoAlbumViewModelV7 videoAlbumViewModelV = GetCurrentContext.AlbumList.First();
			videoAlbumViewModelV.IsSelected = true;
			videoAlbumViewModelV.SingleClickCommand.Execute(true);
		}
		foreach (VideoAlbumViewModelV7 album in GetCurrentContext.AlbumList)
		{
			album.IsSelectedAllVideo = false;
		}
	}

	private void RefreshAblumsListBox()
	{
		txbSearch.Text = string.Empty;
		GetCurrentContext.IsSelectedAllAlbumVideo = false;
		IEnumerable<VideoAlbumViewModelV7> enumerable = GetCurrentContext.AlbumsOriginal.Where((VideoAlbumViewModelV7 m) => m.IsInternalPath == GetCurrentContext.VideoToolRefreshBtn.IsEnabled);
		if (enumerable.FirstOrDefault() != null)
		{
			enumerable.FirstOrDefault().IsSelected = true;
		}
		GetCurrentContext.FocusedAlbum = enumerable.FirstOrDefault();
		GetCurrentContext.AlbumList = new ObservableCollection<VideoAlbumViewModelV7>(enumerable);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
