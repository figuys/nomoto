using System;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public class DownloadEventArgs : EventArgs
{
	private DownloadTask m_task;

	public DownloadTask downloadTask => m_task;

	public DownloadEventArgs(DownloadTask task)
	{
		m_task = task;
	}
}
