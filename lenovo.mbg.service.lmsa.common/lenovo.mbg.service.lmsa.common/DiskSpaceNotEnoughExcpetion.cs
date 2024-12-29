using System;

namespace lenovo.mbg.service.lmsa.common;

public class DiskSpaceNotEnoughExcpetion : Exception
{
	public DiskSpaceNotEnoughExcpetion(string message)
		: base(message)
	{
	}

	public DiskSpaceNotEnoughExcpetion(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
