using System;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;

public class TimeoutException : Exception
{
	public TimeoutException(string exception)
		: base(exception)
	{
	}
}
