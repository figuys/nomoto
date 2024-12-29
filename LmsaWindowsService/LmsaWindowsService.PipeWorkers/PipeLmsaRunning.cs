using System;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService.PipeWorkers;

public class PipeLmsaRunning : IPipeMessageWorker
{
	public static event EventHandler<object> OnNotify;

	public void Do(object data)
	{
		PipeLmsaRunning.OnNotify?.Invoke(null, data);
	}
}
