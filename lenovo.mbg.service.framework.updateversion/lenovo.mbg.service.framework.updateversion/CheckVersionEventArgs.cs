using System;

namespace lenovo.mbg.service.framework.updateversion;

public class CheckVersionEventArgs : EventArgs
{
	private CheckVersionStatus m_status;

	private object m_data;

	public CheckVersionStatus Status => m_status;

	public object Data => m_data;

	public bool IsAutoMode { get; private set; }

	public CheckVersionEventArgs(bool isAutoMode, CheckVersionStatus status)
	{
		m_status = status;
		IsAutoMode = isAutoMode;
	}

	public CheckVersionEventArgs(bool isAutoMode, CheckVersionStatus status, object data)
	{
		m_status = status;
		m_data = data;
		IsAutoMode = isAutoMode;
	}
}
