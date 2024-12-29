using System;

namespace lenovo.mbg.service.framework.services.Device;

[Serializable]
[Flags]
public enum ConnectType
{
	Adb = 1,
	Fastboot = 2,
	Wifi = 4,
	UNKNOW = 5
}
