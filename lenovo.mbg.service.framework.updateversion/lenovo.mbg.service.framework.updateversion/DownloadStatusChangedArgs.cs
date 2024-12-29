using System;

namespace lenovo.mbg.service.framework.updateversion;

public class DownloadStatusChangedArgs : EventArgs
{
	private object m_data;

	private VersionDownloadStatus m_status;

	public object Data => m_data;

	public VersionDownloadStatus Status => m_status;

	public DownloadStatusChangedArgs(object data)
	{
		m_data = data;
	}

	public DownloadStatusChangedArgs(VersionDownloadStatus status)
	{
		m_status = status;
	}

	public DownloadStatusChangedArgs(VersionDownloadStatus status, object data)
	{
		m_status = status;
		m_data = data;
	}
}
