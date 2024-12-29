namespace lenovo.mbg.service.common.utilities;

public class AsyncTaskCancelHander : IAsyncTaskCancelHander
{
	private IAsyncTaskContext context;

	public AsyncTaskCancelHander(IAsyncTaskContext context)
	{
		this.context = context;
	}

	public void Cancel()
	{
		context.Cancel();
	}
}
