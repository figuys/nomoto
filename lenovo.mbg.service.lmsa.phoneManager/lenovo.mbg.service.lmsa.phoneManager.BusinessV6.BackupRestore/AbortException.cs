using System;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class AbortException : BackupRestoreException
{
	public AbortException(string message)
		: base(message)
	{
	}

	public AbortException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
