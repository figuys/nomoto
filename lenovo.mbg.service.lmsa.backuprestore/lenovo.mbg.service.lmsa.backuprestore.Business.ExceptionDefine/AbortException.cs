using System;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;

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
