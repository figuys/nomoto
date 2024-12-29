using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.ResourcesCleanUp;

namespace lenovo.mbg.service.lmsa.Services;

public class ResourcesLoggingService : IResourcesLoggingService
{
	public void RegisterDir(string path)
	{
		ResourcesLog.Single.AddDirRecord(path);
	}

	public void RegisterFile(string path)
	{
		ResourcesLog.Single.AddFileRecord(path);
	}
}
