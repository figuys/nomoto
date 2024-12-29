using System;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;

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
