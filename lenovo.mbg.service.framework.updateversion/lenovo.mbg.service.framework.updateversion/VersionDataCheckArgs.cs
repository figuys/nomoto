using System;

namespace lenovo.mbg.service.framework.updateversion;

public class VersionDataCheckArgs : EventArgs
{
	private VersionDataCheckStatus m_status;

	private object m_data;

	public VersionDataCheckStatus Status => m_status;

	public object Data => m_data;

	public VersionDataCheckArgs(VersionDataCheckStatus status)
	{
		m_status = status;
	}

	public VersionDataCheckArgs(VersionDataCheckStatus status, object data)
	{
		m_status = status;
		m_data = data;
	}
}
