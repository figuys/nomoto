using System;

namespace lenovo.mbg.service.framework.updateversion;

public class VersionUnInstallEventArgs : EventArgs
{
	private object m_data;

	private ServiceUnInstallStatus m_status;

	public object Data => m_data;

	public ServiceUnInstallStatus Status => m_status;

	public VersionUnInstallEventArgs(ServiceUnInstallStatus status)
	{
		m_status = status;
	}

	public VersionUnInstallEventArgs(ServiceUnInstallStatus status, object data)
	{
		m_status = status;
		m_data = data;
	}
}
