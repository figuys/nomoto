using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class PICMgtView : UserControl, IComponentConnector, IStyleConnector
{
	private int startIndex;

	private DleayTask mDelayTask2 = new DleayTask();

	private DleayTask mDelayTask = new DleayTask();

	public PICMgtView()
	{
		InitializeComponent();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}

	private void DateScrolls_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	private void PicScrolls_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	private void AlbumScrolls_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	private void DateScrolls_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		mDelayTask2.ReplacePrevTaskAndStart(1000, delegate(object args)
		{
			if (PicMgtViewModel.SingleInstance.PicListIsLoad)
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					if (((ScrollChangedEventArgs)args).OriginalSource is ScrollViewer scrollViewer)
					{
						PicAlbumViewModel focusedAlbum = PicMgtViewModel.SingleInstance.FocusedAlbum;
						if (focusedAlbum != null)
						{
							try
							{
								focusedAlbum.LoadPicThumbnailList(scrollViewer.ExtentHeight, scrollViewer.ExtentWidth, scrollViewer.VerticalOffset, scrollViewer.ViewportHeight, scrollViewer.ViewportWidth);
							}
							catch (Exception ex)
							{
								LogHelper.LogInstance.Error("Load pic thubnail list error:" + ex);
							}
						}
					}
				});
			}
		}, e);
	}

	private void PicScrolls_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		mDelayTask.ReplacePrevTaskAndStart(1000, delegate(object args)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				if (((ScrollChangedEventArgs)args).OriginalSource is ScrollViewer scrollViewer)
				{
					PicAlbumViewModel focusedAlbum = PicMgtViewModel.SingleInstance.FocusedAlbum;
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
}
