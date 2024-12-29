using System;

namespace lenovo.mbg.service.framework.services.Device;

public class MasterDeviceChangedEventArgs : EventArgs
{
	public DeviceEx Previous { get; private set; }

	public DeviceEx Current { get; private set; }

	public MasterDeviceChangedEventArgs(DeviceEx previous, DeviceEx current)
	{
		Previous = previous;
		Current = current;
	}
}
