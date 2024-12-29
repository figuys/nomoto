using System;
using System.Threading.Tasks;

namespace lenovo.mbg.service.common.utilities;

public class AsyncTaskRunner
{
	private IAsyncTaskContext context;

	private IAsyncTaskCancelHander cancelHandler;

	public static IAsyncTaskCancelHander BeginInvok(Action<IAsyncTaskContext> task, Action<IAsyncTaskResult> callback, object objectState)
	{
		return BeginInvok(task, callback, objectState, null);
	}

	public static IAsyncTaskCancelHander BeginInvok(Action<IAsyncTaskContext> task, Action<IAsyncTaskResult> callback, object objectState, TaskScheduler scheduler)
	{
		IAsyncTaskContext context = new AsyncTaskContext(objectState);
		IAsyncTaskCancelHander result = new AsyncTaskCancelHander(context);
		Task task2 = new Task(delegate
		{
			Exception exception = null;
			try
			{
				task(context);
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			callback?.Invoke(new AsyncTaskResult(context, exception));
		});
		if (scheduler == null)
		{
			task2.Start();
		}
		else
		{
			task2.Start(scheduler);
		}
		return result;
	}

	public IAsyncTaskCancelHander Init(object objectState)
	{
		context = new AsyncTaskContext(objectState);
		cancelHandler = new AsyncTaskCancelHander(context);
		return cancelHandler;
	}

	public IAsyncTaskCancelHander BeginInvok(Action<IAsyncTaskContext> task, Action<IAsyncTaskResult> callback)
	{
		Task.Factory.StartNew(delegate
		{
			Exception exception = null;
			try
			{
				task(context);
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			callback?.Invoke(new AsyncTaskResult(context, exception));
		});
		return cancelHandler;
	}
}
