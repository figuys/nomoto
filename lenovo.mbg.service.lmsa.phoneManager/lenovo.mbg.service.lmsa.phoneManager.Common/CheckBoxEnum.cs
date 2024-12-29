using System;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

[Flags]
public enum CheckBoxEnum
{
	None = 0,
	Today = 1,
	YesterDay = 2,
	TwoDaysAgo = 4,
	ThreeDaysAgo = 8,
	AllDays = 0x10
}
