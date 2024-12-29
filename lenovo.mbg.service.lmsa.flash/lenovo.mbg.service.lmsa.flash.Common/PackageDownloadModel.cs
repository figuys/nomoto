using System.Collections.Generic;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class PackageDownloadModel
{
	public List<DownloadInfo> Resources { get; private set; }

	public string ModelName { get; private set; }

	public PackageDownloadModel()
	{
		Resources = new List<DownloadInfo>();
	}

	public PackageDownloadModel(string modelname)
		: this()
	{
		ModelName = modelname;
	}

	public void Add(DownloadInfo resource)
	{
		if (resource != null)
		{
			Resources.Add(resource);
		}
	}
}
