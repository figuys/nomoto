using System;
using System.Collections.Concurrent;

namespace lenovo.mbg.service.common.utilities;

public class AsyncTaskContext : IAsyncTaskContext, ICancel
{
	private ConcurrentBag<object> cancelerList = new ConcurrentBag<object>();

	private volatile bool isCancelCommandRequested;

	private volatile bool isCanceled;

	private volatile bool canceling;

	public object ObjectState { get; private set; }

	public bool IsCancelCommandRequested => isCancelCommandRequested;

	public bool IsCanceled => isCanceled;

	public AsyncTaskContext(object objectState)
	{
		ObjectState = objectState;
	}

	public void AddCancelSource(ICancel canceler)
	{
		cancelerList.Add(canceler);
	}

	public void AddCancelSource(Action canceler)
	{
		cancelerList.Add(canceler);
	}

	public void ResetCancelStatus()
	{
		isCancelCommandRequested = false;
		isCanceled = false;
	}

	public void Cancel()
	{
		lock (this)
		{
			if (canceling)
			{
				return;
			}
			canceling = true;
		}
		isCancelCommandRequested = true;
		try
		{
			while (!cancelerList.IsEmpty)
			{
				if (cancelerList.TryTake(out var result) && result != null)
				{
					if (result is Action action)
					{
						action();
					}
					else if (result is ICancel cancel)
					{
						cancel.Cancel();
					}
				}
			}
			isCanceled = true;
		}
		finally
		{
			canceling = false;
		}
	}
}
