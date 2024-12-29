using System;

namespace lenovo.mbg.service.framework.socket;

public class HeartbeatStoppedEventArgs : EventArgs
{
	public bool IsDisconnectedByService { get; set; }

	public bool IsTimeout { get; set; }

	public HeartbeatStoppedEventArgs(bool isDisconnectedByService, bool isTimeout)
	{
		IsDisconnectedByService = isDisconnectedByService;
		IsTimeout = isTimeout;
	}
}
