using System;

namespace lenovo.mbg.service.common.utilities;

public interface IAsyncTaskContext : ICancel
{
	object ObjectState { get; }

	bool IsCancelCommandRequested { get; }

	bool IsCanceled { get; }

	void AddCancelSource(Action canceler);

	void AddCancelSource(ICancel canceler);

	void ResetCancelStatus();
}
