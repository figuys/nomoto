using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

namespace lenovo.mbg.service.lmsa.toolbox;

public partial class ToolboxFrameV6 : UserControl, IComponentConnector
{
	private ToolBoxListViewModelV6 _VM;

	public ToolboxFrameV6()
	{
		InitializeComponent();
		_VM = new ToolBoxListViewModelV6();
		base.DataContext = _VM;
		base.Loaded += delegate
		{
			HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
		};
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		base.Dispatcher.Invoke(delegate
		{
			if (_VM.CurrentView is ClipboardUserControlV6 || _VM.CurrentView is ScreenCaptureFrameV6)
			{
				switch (e)
				{
				case DeviceSoftStateEx.Online:
					ToolboxViewContext.SingleInstance.CloseTutorialsView();
					break;
				case DeviceSoftStateEx.Offline:
					ToolboxViewContext.SingleInstance.ShowTutorialsView();
					break;
				}
			}
		});
	}

	private void ConnectButtonClick(object sender, RoutedEventArgs e)
	{
		ToolboxViewContext.SingleInstance.ShowTutorialsView();
	}
}
