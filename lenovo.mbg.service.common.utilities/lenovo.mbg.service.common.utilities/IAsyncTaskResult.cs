using System;

namespace lenovo.mbg.service.common.utilities;

public interface IAsyncTaskResult
{
	object ObjectState { get; }

	Exception Exception { get; }

	bool IsCanceled { get; }

	bool IsCancelCommandRequested { get; }
}
