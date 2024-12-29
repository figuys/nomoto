using lenovo.mbg.service.framework.services.Device;
using SharpAdbClient;

namespace lenovo.mbg.service.framework.devicemgt;

public class DeviceDataEx
{
	public DevicePhysicalStateEx PhyState { get; set; }

	public DeviceData Data { get; set; }
}
