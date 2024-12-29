using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class PICMgtViewV7 : UserControl, IComponentConnector, IStyleConnector
{
	private int startIndex;

	private DleayTask mDelayTask = new DleayTask();

	private PicMgtViewModelV7 GetCurrentContext
	{
		get
		{
			PicMgtViewModelV7 _dataContext = null;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				_dataContext = base.DataContext as PicMgtViewModelV7;
			});
			return _dataContext;
		}
	}

	public PICMgtViewV7()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			sdview.Cbx.SelectionChanged -= ddlStorageSelect_SelectionChanged;
			sdview.Cbx.SelectionChanged += ddlStorageSelect_SelectionChanged;
			if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto" && GetCurrentContext != null)
			{
				rbCheckCard.IsChecked = true;
				GetCurrentContext.StorageSelectPanelVisibility = Visibility.Collapsed;
			}
			else
			{
				GetCurrentContext.StorageSelectPanelVisibility = Visibility.Visible;
			}
		};
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}

	private void PicScrolls_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	private void PicScrolls_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		mDelayTask.ReplacePrevTaskAndStart(1000, delegate(object args)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				if (((ScrollChangedEventArgs)args).OriginalSource is ScrollViewer scrollViewer)
				{
					PicAlbumViewModelV7 focusedAlbum = GetCurrentContext.FocusedAlbum;
					if (focusedAlbum != null)
					{
						double num = Application.Current.MainWindow.ActualWidth - 200.0;
						double num2 = Application.Current.MainWindow.Height - 200.0;
						int num3 = 115;
						int num4 = 115;
						int num5 = (int)(num2 / (double)num3) + ((num2 % (double)num3 > 0.0) ? 1 : 0);
						int num6 = (int)(num / (double)num4) + ((num % (double)num4 > 0.0) ? 1 : 0);
						if (scrollViewer.VerticalOffset > 10.0)
						{
							startIndex = num5 * (int)Math.Floor(scrollViewer.VerticalOffset / (double)num3);
							focusedAlbum.CountPicIds(startIndex, startIndex + num5 * num6);
						}
					}
				}
			});
		}, e);
	}

	private void OnCheckBoxSelectAllAlbums(object sender, RoutedEventArgs e)
	{
		bool isSelected = (sender as CheckBox).IsChecked == true;
		SelectAllAblumsPictures(isSelected);
	}

	private void SelectAllItemClick(object sender, RoutedEventArgs e)
	{
		SelectAllAblumsPictures(_isSelected: true);
	}

	private void UnselectAllItemClick(object sender, RoutedEventArgs e)
	{
		SelectAllAblumsPictures(_isSelected: false);
	}

	private void SelectAllAblumsPictures(bool _isSelected)
	{
		foreach (PicAlbumViewModelV7 album in GetCurrentContext.Albums)
		{
			if (album.CachedAllPics.Count == 0)
			{
				album.SingleClickCommand.Execute(true);
			}
			album.IsSelectedAllPic = _isSelected;
		}
		GetCurrentContext.RefreshAllAlbumPicSelectedCount();
	}

	private void OnCheckBoxSelectAllInAlbum(object sender, RoutedEventArgs e)
	{
		if (GetCurrentContext.FocusedAlbum?.CachedAllPics == null)
		{
			return;
		}
		CheckBox obj = sender as CheckBox;
		ObservableCollection<PicInfoViewModelV7> cachedAllPics = GetCurrentContext.FocusedAlbum.CachedAllPics;
		bool isSelected = obj.IsChecked == true;
		foreach (PicInfoViewModelV7 item in cachedAllPics)
		{
			item.IsSelected = isSelected;
		}
	}

	private void ddlStorageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (GetCurrentContext?.PicToolImportBtn == null)
		{
			return;
		}
		if (GetCurrentContext.IsSelectedInternal)
		{
			GetCurrentContext.PicToolImportBtn.IsEnabled = true;
		}
		else
		{
			GetCurrentContext.PicToolImportBtn.IsEnabled = false;
		}
		GetCurrentContext.PicToolRefreshCommandHandler(null);
		foreach (PicAlbumViewModelV7 album in GetCurrentContext.Albums)
		{
			album.IsSelectedAllPic = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
