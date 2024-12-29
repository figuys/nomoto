using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

public partial class ScreenCaptureFrameV6 : UserControl, IDisposable, IComponentConnector, IStyleConnector
{
	private ScreenCaptureFrameViewModeV6 _VM;

	private ScreenCaptureFrameViewModeV6 GetCurrentContext => _VM;

	public ScreenCaptureFrameV6()
	{
		InitializeComponent();
		_VM = new ScreenCaptureFrameViewModeV6();
		base.DataContext = _VM;
		base.Loaded += delegate
		{
			DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
			if (masterDevice == null || masterDevice.SoftStatus != DeviceSoftStateEx.Online)
			{
				ToolboxViewContext.SingleInstance.ShowTutorialsView();
			}
		};
	}

	public void Dispose()
	{
		_VM.Dispose();
		_VM = null;
	}

	private void ViewButtonClick(object sender, RoutedEventArgs e)
	{
	}

	private void VideoTileList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer obj = (ScrollViewer)sender;
		obj.ScrollToVerticalOffset(obj.VerticalOffset - (double)(e.Delta / 2));
		e.Handled = true;
	}

	private void CheckAllClick(object sender, RoutedEventArgs e)
	{
		if (sender is CheckBox { IsChecked: not null } checkBox)
		{
			bool _checked = checkBox.IsChecked.Value;
			GetCurrentContext.VideoDataList.All(delegate(CaptureVideoItemDetailViewModelV6 v)
			{
				v.IsChecked = _checked;
				return true;
			});
			LangButton langButton = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = GetCurrentContext.VideoDataList.Count != 0 && checkBox.IsChecked.Value);
			langButton.IsEnabled = isEnabled;
			if (!checkBox.IsChecked.Value && lbTileView.SelectedItems.Count > 0)
			{
				lbTileView.SelectedIndex = -1;
			}
		}
	}

	private void CellCheckBoxClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is CheckBox { IsChecked: not null, IsChecked: var isChecked }))
		{
			return;
		}
		if (isChecked == true)
		{
			LangButton langButton = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = true);
			langButton.IsEnabled = isEnabled;
			GetCurrentContext.IsAllSelected = !GetCurrentContext.VideoDataList.Any((CaptureVideoItemDetailViewModelV6 p) => !p.IsChecked);
		}
		else
		{
			LangButton langButton2 = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = GetCurrentContext.VideoDataList.Any((CaptureVideoItemDetailViewModelV6 p) => p.IsChecked));
			langButton2.IsEnabled = isEnabled;
			GetCurrentContext.IsAllSelected = false;
		}
	}

	private void lbTileVideoSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (GetCurrentContext.VideoDataList.Any((CaptureVideoItemDetailViewModelV6 v) => v.IsChecked))
		{
			LangButton langButton = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = true);
			langButton.IsEnabled = isEnabled;
			GetCurrentContext.IsAllSelected = !GetCurrentContext.VideoDataList.Any((CaptureVideoItemDetailViewModelV6 p) => !p.IsChecked);
		}
		else
		{
			LangButton langButton2 = btnExport;
			bool isEnabled = (btnDelete.IsEnabled = false);
			langButton2.IsEnabled = isEnabled;
			GetCurrentContext.IsAllSelected = false;
		}
	}
}
