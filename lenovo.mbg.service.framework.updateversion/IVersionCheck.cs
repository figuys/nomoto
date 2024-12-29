using System;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionCheck
{
	IVersionData VersionData { get; }

	event EventHandler<CheckVersionEventArgs> OnCheckVersionStatusChanged;

	void Check(bool isAutoMode);
}
