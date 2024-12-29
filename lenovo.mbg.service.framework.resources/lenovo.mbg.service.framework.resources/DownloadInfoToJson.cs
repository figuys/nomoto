using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.framework.resources;

public class DownloadInfoToJson : ISaveDownloadInfo
{
	public string DownloadingPath => Configurations.DownloadingSavePath;

	public string DownloadedPath => Configurations.DownloadedSavePath;

	public bool Save(string path, object data)
	{
		return JsonHelper.SerializeObject2File(path, data);
	}

	public List<T> Get<T>(string path)
	{
		return JsonHelper.DeserializeJson2ListFromFile<T>(path);
	}
}
