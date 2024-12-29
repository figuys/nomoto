using System.Collections.Generic;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.framework.services;

public interface IRsd
{
	DownloadInfo GetDownloadedResource(string downloadurl, out string filePath);

	Dictionary<string, List<DownloadInfo>> LoadCopyRomResources(List<string> desriptionFilePaths);

	Dictionary<string, List<DownloadInfo>> ValidateCopyRomResources(Dictionary<string, List<DownloadInfo>> sources);

	int Unzip(DownloadInfo downloadInfo);
}
