using System;

namespace lenovo.themes.generic.Exceptions;

public class CacnelException : Exception
{
	public CacnelException(string message)
		: base(message)
	{
	}

	public CacnelException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
