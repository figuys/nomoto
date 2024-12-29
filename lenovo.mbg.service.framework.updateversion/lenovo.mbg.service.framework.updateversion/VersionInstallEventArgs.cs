using System;

namespace lenovo.mbg.service.framework.updateversion;

public class VersionInstallEventArgs : EventArgs
{
	private object m_data;

	private ServiceInstallStatus m_status;

	public object Data => m_data;

	public ServiceInstallStatus Status => m_status;

	public VersionInstallEventArgs(ServiceInstallStatus status)
	{
		m_status = status;
	}

	public VersionInstallEventArgs(ServiceInstallStatus status, object data)
	{
		m_status = status;
		m_data = data;
	}
}
