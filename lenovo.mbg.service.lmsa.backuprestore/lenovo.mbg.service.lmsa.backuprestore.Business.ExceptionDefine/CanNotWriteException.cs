using System;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;

public class CanNotWriteException : Exception
{
	public CanNotWriteException(string message)
		: base(message)
	{
	}

	public CanNotWriteException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
