namespace lenovo.mbg.service.framework.services;

public interface IResourcesLoggingService
{
	void RegisterFile(string path);

	void RegisterDir(string path);
}
