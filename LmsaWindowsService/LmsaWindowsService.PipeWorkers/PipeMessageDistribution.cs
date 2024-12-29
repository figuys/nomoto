using System.Collections.Generic;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.pipes;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService.PipeWorkers;

public class PipeMessageDistribution
{
	public Dictionary<PipeMessage, string> _Cached = new Dictionary<PipeMessage, string>
	{
		{
			PipeMessage.LMSA_UPLOAD_CONFIRM,
			"LmsaWindowsService.PipeWorkers.PipeLmsaUploadConfirm"
		},
		{
			PipeMessage.LMSA_RUNNING,
			"LmsaWindowsService.PipeWorkers.PipeLmsaRunning"
		},
		{
			PipeMessage.LMSA_FULL_INSTALL,
			"LmsaWindowsService.PipeWorkers.PipeLmsaFullInstall"
		},
		{
			PipeMessage.LMSA_INCREMENT_INSTALL,
			"LmsaWindowsService.PipeWorkers.PipeLmsaIncrementInstall"
		},
		{
			PipeMessage.LMSA_DATA,
			"LmsaWindowsService.PipeWorkers.PipeLmsaData"
		}
	};

	public void Received(PipeMessage message, object data)
	{
		IPipeMessageWorker _worker = PipeWorkerFactory.CreateInstance<IPipeMessageWorker>(_Cached[message]);
		Task.Factory.StartNew(delegate
		{
			_worker?.Do(data);
		});
	}
}
