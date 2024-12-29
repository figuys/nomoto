using System;

namespace lenovo.mbg.service.framework.services.Device;

[Serializable]
public enum DeviceLockReason
{
	None,
	Reboot2Recovery,
	Reboot2Fastboot
}
