using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Download;

public interface IFileDownload
{
	int DownloadingCount { get; }

	event EventHandler<RemoteDownloadStatusEventArgs> OnRemoteDownloadStatusChanged;

	Dictionary<DownloadInfoType, List<DownloadInfo>> Load();

	void Add(DownloadInfo resource, bool autoStart = true);

	void Add(List<DownloadInfo> resources, bool autoStart = true);

	void ReStart(string url);

	void Stop(string url);

	void Stop();

	void Delete(string url);

	DownloadInfo GetDownloadedResource(string url);

	DownloadInfo GetDownloadingResource(string url);
}
