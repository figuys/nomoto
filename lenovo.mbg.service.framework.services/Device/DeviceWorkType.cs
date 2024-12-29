using System;

namespace lenovo.mbg.service.framework.services.Device;

[Flags]
public enum DeviceWorkType : uint
{
	None = 1u,
	Rescue = 2u,
	ReadFastboot = 4u
}
