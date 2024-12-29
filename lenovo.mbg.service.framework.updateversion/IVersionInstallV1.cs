using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionInstallV1 : IVersionEvent
{
	VersionModel Model { get; }

	void Install();
}
