using System.Threading;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadMode;

public interface IDownload
{
	DownloadStatus Start(DownloadTask task, CancellationTokenSource tokeSource);
}
