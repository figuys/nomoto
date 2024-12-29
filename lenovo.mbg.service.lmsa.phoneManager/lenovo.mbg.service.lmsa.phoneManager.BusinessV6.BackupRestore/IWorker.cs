using System;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public interface IWorker : IDisposable
{
	int WorkerSequence { get; }

	IAsyncTaskContext TaskContext { get; }

	string WorkerId { get; }

	void DoProcess(object state);

	void Cancel();

	void Abort();
}
