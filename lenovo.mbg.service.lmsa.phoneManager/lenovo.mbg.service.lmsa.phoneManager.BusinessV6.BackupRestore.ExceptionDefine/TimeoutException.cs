using System;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore.ExceptionDefine;

public class TimeoutException : Exception
{
	public TimeoutException(string exception)
		: base(exception)
	{
	}
}
