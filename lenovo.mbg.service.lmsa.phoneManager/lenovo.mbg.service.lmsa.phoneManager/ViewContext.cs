using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager;

public class ViewContext
{
	private static ViewContext _singleInstance;

	public static ViewContext SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new ViewContext();
			}
			return _singleInstance;
		}
	}

	public static DeviceEx MasterDevice => HostProxy.deviceManager.MasterDevice;

	public MainViewModel MainViewModel { get; set; }

	private ViewContext()
	{
	}
}
