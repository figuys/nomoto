using System;
using System.Text;
using System.Threading;

namespace lenovo.mbg.service.common.log;

public class BusinessLog
{
	private readonly object locker = new object();

	public StringBuilder LogCache { get; private set; }

	public void Write(string method, string message, LogLevel level, Exception exception)
	{
		string message2 = $"{$"{DateTime.Now:G}"} [{Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(2)}] [{level.ToString().PadLeft(5)}] {method} - {message}";
		Write(message2, exception);
	}

	public void Clear()
	{
		lock (locker)
		{
			LogCache.Clear();
		}
	}

	public override string ToString()
	{
		lock (locker)
		{
			return LogCache.ToString();
		}
	}

	public void Write(string message, Exception exception)
	{
		lock (locker)
		{
			LogCache.AppendLine(message);
			if (exception != null)
			{
				LogCache.AppendLine(exception.ToString());
			}
		}
	}

	public BusinessLog()
	{
		LogCache = new StringBuilder();
	}
}
