using System;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.backuprestore.Business;

public interface IWorker : IDisposable
{
	int WorkerSequence { get; }

	IAsyncTaskContext TaskContext { get; }

	string WorkerId { get; }

	void DoProcess(object state);

	void Cancel();

	void Abort();
}
