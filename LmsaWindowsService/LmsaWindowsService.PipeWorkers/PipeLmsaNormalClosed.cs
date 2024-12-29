using LmsaWindowsService.Contexts;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService.PipeWorkers;

public class PipeLmsaNormalClosed : IPipeMessageWorker
{
	public void Do(object data)
	{
		ServiceContext.LmsaNormalColsed = true;
	}
}
