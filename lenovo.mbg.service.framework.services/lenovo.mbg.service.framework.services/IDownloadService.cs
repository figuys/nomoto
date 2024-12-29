using System;

namespace lenovo.mbg.service.framework.services;

public interface IDownloadService : IDisposable
{
	event EventHandler<DownloadEventArgs> OnDownloadStatusChanged;
}
