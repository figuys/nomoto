using System;

namespace lenovo.mbg.service.framework.devicemgt;

public class TcpConnectStepChangedEventArgs : EventArgs
{
	public string Step { get; private set; }

	public ConnectStepStatus Result { get; private set; }

	public int Percent { get; private set; }

	public ConnectErrorCode ErrorCode { get; set; }

	public TcpConnectStepChangedEventArgs(string step, ConnectStepStatus result, ConnectErrorCode errorCode, int percent)
	{
		Step = step;
		Result = result;
		Percent = percent;
		ErrorCode = errorCode;
	}
}
