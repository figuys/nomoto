using System;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionDownload
{
	event EventHandler<DownloadStatusChangedArgs> OnDownloadStatusChanged;

	void Cancel();

	void DownloadVersion(object data);
}
