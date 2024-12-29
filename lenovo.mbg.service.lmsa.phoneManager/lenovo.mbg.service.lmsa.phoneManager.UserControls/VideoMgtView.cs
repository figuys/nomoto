using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class VideoMgtView : UserControl, IComponentConnector, IStyleConnector
{
	public VideoMgtView()
	{
		InitializeComponent();
		if (VideoMgtViewModel.SingleInstance.AlbumViewCallback == null)
		{
			VideoMgtViewModel.SingleInstance.AlbumViewCallback = AlbumViewCallbackHandler;
		}
		VideoMgtViewModel.SingleInstance._defaultImage = FindResource("DefVedioDrawingImage") as ImageSource;
		VideoMgtViewModel.SingleInstance.OnDeleteEvent = SetButtonState;
		base.DataContext = VideoMgtViewModel.SingleInstance;
		TopCategory.SelectedIndex = 0;
	}

	protected void AlbumViewCallbackHandler(VideoViewType videoViewType)
	{
		switch (videoViewType)
		{
		case VideoViewType.Video:
			ToAlbumVideo();
			break;
		case VideoViewType.Album:
			BackToAlbum(null, null);
			break;
		}
	}

	private void VideoAlbumViewChange(object sender, SelectionChangedEventArgs e)
	{
		VideoMgtViewModel videoMgtViewModel = base.DataContext as VideoMgtViewModel;
		if (TopCategory.SelectedIndex == 0)
		{
			VideoView.Visibility = Visibility.Visible;
			AlbumView.Visibility = Visibility.Hidden;
			spForVideo.Visibility = Visibility.Visible;
			btnBack.Visibility = Visibility.Collapsed;
			CheckAllState();
			videoMgtViewModel.MainView = VideoViewType.Video;
			videoMgtViewModel.UpdateVideoList();
		}
		else if (TopCategory.SelectedIndex == 1)
		{
			if (videoMgtViewModel.AlbumView == VideoViewType.Album)
			{
				VideoView.Visibility = Visibility.Hidden;
				AlbumView.Visibility = Visibility.Visible;
				spForVideo.Visibility = Visibility.Hidden;
				btnBack.Visibility = Visibility.Collapsed;
				checkAll.Visibility = Visibility.Collapsed;
			}
			else
			{
				CheckAllState();
				btnBack.Visibility = Visibility.Visible;
				videoMgtViewModel.UpdateAlbumVideoList();
			}
			videoMgtViewModel.MainView = VideoViewType.Album;
		}
		videoMgtViewModel.IsAllSelected = !videoMgtViewModel.VideoList.Any((VideoInfoViewModel p) => !p.IsSelected);
		SetButtonState();
	}

	private void CheckAllClick(object sender, RoutedEventArgs e)
	{
		CheckBox cbAll = sender as CheckBox;
		if (cbAll == null || !cbAll.IsChecked.HasValue)
		{
			return;
		}
		VideoMgtViewModel.SingleInstance.VideoList.All(delegate(VideoInfoViewModel v)
		{
			v.IsSelected = cbAll.IsChecked.Value;
			return true;
		});
		IconButton iconButton = btnExport;
		bool isEnabled = (btnDelete.IsEnabled = VideoMgtViewModel.SingleInstance.VideoList.Count != 0 && cbAll.IsChecked.Value);
		iconButton.IsEnabled = isEnabled;
		if (!cbAll.IsChecked.Value && lbTileView.SelectedItems.Count > 0)
		{
			lbTileView.SelectedIndex = -1;
		}
		if (VideoMgtViewModel.SingleInstance.MainView == VideoViewType.Video)
		{
			if (cbAll.IsChecked.Value)
			{
				VideoMgtViewModel.SingleInstance.VideosChoosed = ((VideoMgtViewModel.SingleInstance.VideoList != null) ? VideoMgtViewModel.SingleInstance.VideoList.Count : 0);
			}
			else
			{
				VideoMgtViewModel.SingleInstance.VideosChoosed = 0;
			}
		}
		else if (VideoMgtViewModel.SingleInstance.AlbumView == VideoViewType.Video)
		{
			int videosChoosed = (from n in VideoMgtViewModel.SingleInstance.AlbumList.SelectMany((VideoAlbumViewModel n) => n.VideoInfoList)
				where n.IsSelected
				select n).Count();
			VideoMgtViewModel.SingleInstance.VideosChoosed = videosChoosed;
		}
	}

	private void CellCheckBoxClick(object sender, RoutedEventArgs e)
	{
		if (sender is CheckBox { IsChecked: not null } checkBox)
		{
			VideoMgtViewModel videoMgtViewModel = base.DataContext as VideoMgtViewModel;
			if (checkBox.IsChecked == true)
			{
				VideoMgtViewModel.SingleInstance.VideosChoosed++;
				videoMgtViewModel.IsAllSelected = !videoMgtViewModel.VideoList.Any((VideoInfoViewModel p) => !p.IsSelected);
			}
			else
			{
				videoMgtViewModel.IsAllSelected = false;
				VideoMgtViewModel.SingleInstance.VideosChoosed--;
			}
		}
		SetButtonState();
	}

	public void SetButtonState()
	{
		if (VideoMgtViewModel.SingleInstance.VideoList.Count == 0 || VideoMgtViewModel.SingleInstance.VideoList.Any((VideoInfoViewModel a) => !a.IsSelected))
		{
			VideoMgtViewModel.SingleInstance.IsAllSelected = false;
		}
		else
		{
			VideoMgtViewModel.SingleInstance.IsAllSelected = true;
		}
		if (VideoMgtViewModel.SingleInstance.MainView == VideoViewType.Video || VideoMgtViewModel.SingleInstance.AlbumView == VideoViewType.Video)
		{
			if (VideoMgtViewModel.SingleInstance.VideoList.Any((VideoInfoViewModel v) => v.IsSelected))
			{
				IconButton iconButton = btnExport;
				bool isEnabled = (btnDelete.IsEnabled = true);
				iconButton.IsEnabled = isEnabled;
			}
			else
			{
				IconButton iconButton2 = btnExport;
				bool isEnabled = (btnDelete.IsEnabled = false);
				iconButton2.IsEnabled = isEnabled;
			}
		}
		else
		{
			IconButton iconButton3 = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = false);
			iconButton3.IsEnabled = isEnabled;
		}
	}

	private void Sort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
	}

	private void AlbumListboxMouseDoubleClick(object sender, RoutedEventArgs e)
	{
		ListBoxItem listBoxItem = sender as ListBoxItem;
		VideoMgtViewModel.SingleInstance.SelectedAlbum = listBoxItem.DataContext as VideoAlbumViewModel;
		VideoMgtViewModel.SingleInstance.UpdateAlbumVideoList();
		ToAlbumVideo();
		SetButtonState();
	}

	private void BackToAlbum(object sender, RoutedEventArgs e)
	{
		VideoMgtViewModel.SingleInstance.AlbumView = VideoViewType.Album;
		AlbumView.Visibility = Visibility.Visible;
		spForVideo.Visibility = Visibility.Hidden;
		VideoView.Visibility = Visibility.Hidden;
		btnBack.Visibility = Visibility.Collapsed;
		checkAll.Visibility = Visibility.Collapsed;
		IconButton iconButton = btnExport;
		bool isEnabled = (btnDelete.IsEnabled = false);
		iconButton.IsEnabled = isEnabled;
	}

	private void ToAlbumVideo()
	{
		VideoMgtViewModel.SingleInstance.AlbumView = VideoViewType.Video;
		AlbumView.Visibility = Visibility.Hidden;
		spForVideo.Visibility = Visibility.Visible;
		VideoView.Visibility = Visibility.Visible;
		btnBack.Visibility = Visibility.Visible;
		checkAll.Visibility = ((!rbtTileView.IsChecked.Value) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void ViewButtonClick(object sender, RoutedEventArgs e)
	{
		CheckAllState();
	}

	private void CheckAllState()
	{
		if (rbtTileView.IsChecked.Value)
		{
			checkAll.Visibility = Visibility.Visible;
		}
		else
		{
			checkAll.Visibility = Visibility.Collapsed;
		}
	}

	private void lbTileVideoSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems != null && e.AddedItems.Count > 0)
		{
			foreach (object addedItem in e.AddedItems)
			{
				(addedItem as VideoInfoViewModel).IsSelected = true;
			}
		}
		if (e.RemovedItems != null && e.RemovedItems.Count > 0)
		{
			foreach (object removedItem in e.RemovedItems)
			{
				(removedItem as VideoInfoViewModel).IsSelected = false;
			}
		}
		SetButtonState();
	}

	private void DGVideoListSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		SetButtonState();
	}

	private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ListBox listBox = sender as ListBox;
		VideoMgtViewModel.SingleInstance.AlbumChoosed = ((listBox.SelectedItems != null) ? listBox.SelectedItems.Count : 0);
	}

	private void VideoTileList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)(e.Delta / 2));
		e.Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
