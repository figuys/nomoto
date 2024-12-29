using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class USBConnectingFailViewModel : ConnectingViewModel
{
	public ReplayCommand RetryCommand { get; set; }

	public USBConnectingFailViewModel()
	{
		RetryCommand = new ReplayCommand(delegate
		{
			HostProxy.deviceManager.MasterDevice.PhysicalStatus = DevicePhysicalStateEx.Offline;
		});
	}
}
