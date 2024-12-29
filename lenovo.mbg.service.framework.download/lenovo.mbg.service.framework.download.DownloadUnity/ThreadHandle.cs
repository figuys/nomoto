using System;
using System.Threading;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public class ThreadHandle
{
	public Guid ID = Guid.NewGuid();

	public CancellationTokenSource tokenSource = new CancellationTokenSource();
}
