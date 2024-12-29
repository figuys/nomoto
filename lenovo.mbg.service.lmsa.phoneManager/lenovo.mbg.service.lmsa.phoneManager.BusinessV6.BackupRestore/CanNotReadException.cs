using System;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class CanNotReadException : Exception
{
	public CanNotReadException(string message)
		: base(message)
	{
	}

	public CanNotReadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
