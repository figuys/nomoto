using System;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionUnInstall
{
	event EventHandler<VersionUnInstallEventArgs> OnUnInstallStatusChanged;

	void UnInstallVersion(object data);

	void Cancel();
}
