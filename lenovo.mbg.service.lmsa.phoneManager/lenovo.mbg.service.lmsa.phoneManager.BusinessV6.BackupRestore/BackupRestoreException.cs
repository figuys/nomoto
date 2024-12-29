using System;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupRestoreException : Exception
{
	public BackupRestoreException(string message)
		: base(message)
	{
	}

	public BackupRestoreException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
