using System;

namespace lenovo.mbg.service.framework.updateversion.model;

public class VersionV1EventArgs : EventArgs
{
	public VersionV1Status Status { get; }

	public object Data { get; }

	public VersionV1EventArgs(VersionV1Status status)
		: this(status, null)
	{
	}

	public VersionV1EventArgs(VersionV1Status status, object data)
	{
		Status = status;
		Data = data;
	}
}
