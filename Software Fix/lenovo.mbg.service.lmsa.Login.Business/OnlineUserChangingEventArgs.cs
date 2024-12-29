using System;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class OnlineUserChangingEventArgs : EventArgs
{
	public DateTime Timestamp { get; private set; }

	public bool IsLoggingIn { get; private set; }

	public string UserSource { get; private set; }

	public OnlineUserChangingEventArgs(string userSource, bool isLoggingIn, DateTime timestamp)
	{
		Timestamp = timestamp;
		UserSource = userSource;
		IsLoggingIn = isLoggingIn;
	}
}
