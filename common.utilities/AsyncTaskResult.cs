using System;

namespace lenovo.mbg.service.common.utilities;

public class AsyncTaskResult : IAsyncTaskResult
{
	private IAsyncTaskContext context;

	public Exception Exception { get; private set; }

	public bool IsCanceled => context.IsCanceled;

	public object ObjectState => context.ObjectState;

	public bool IsCancelCommandRequested => context.IsCancelCommandRequested;

	public AsyncTaskResult(IAsyncTaskContext context, Exception exception)
	{
		this.context = context;
		Exception = exception;
	}
}
