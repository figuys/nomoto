using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public interface IPhysicalConnectionListener
{
	void OnConnect(DeviceEx device, DevicePhysicalStateEx phyState);

	void OnDisconnect(DeviceEx device);
}
