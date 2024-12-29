using System;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService.PipeWorkers;

public class PipeLmsaUploadConfirm : IPipeMessageWorker
{
	public static event EventHandler<object> OnConfirm;

	public void Do(object data)
	{
		PipeLmsaUploadConfirm.OnConfirm?.Invoke(null, data);
	}
}
