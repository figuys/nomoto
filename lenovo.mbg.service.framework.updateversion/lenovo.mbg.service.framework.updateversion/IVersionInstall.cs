using System;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionInstall
{
	event EventHandler<VersionInstallEventArgs> OnInstallStatusChanged;

	void InstallVersion(object data);

	void Cancel();
}
