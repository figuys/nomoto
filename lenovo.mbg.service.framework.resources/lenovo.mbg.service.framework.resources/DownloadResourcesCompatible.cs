using System;
using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.resources;

public class DownloadResourcesCompatible
{
	private static string downloading_path = Path.Combine(Configurations.DownloadInfoSavePath, "download.json");

	private static string downloaded_path = Path.Combine(Configurations.DownloadInfoSavePath, "downloaded.json");

	public static List<DownloadInfo> LoadDownloaded()
	{
		return Convert(downloaded_path);
	}

	public static List<DownloadInfo> LoadDownloading()
	{
		return Convert(downloading_path);
	}

	protected static List<DownloadInfo> Convert(string path)
	{
		List<DownloadInfo> list = new List<DownloadInfo>();
		if (GlobalFun.Exists(path))
		{
			List<JObject> list2 = JsonHelper.DeserializeJson2ListFromFile<JObject>(path);
			if (list2 != null && list2.Count > 0)
			{
				foreach (JObject item2 in list2)
				{
					DownloadInfo item = new DownloadInfo
					{
						DownloadUrl = item2.Value<string>("FileUrl"),
						FileType = item2.Value<string>("FileType"),
						LocalPath = item2.Value<string>("LocalPath"),
						MD5 = item2.Value<string>("MD5"),
						ShowInUI = true,
						FileSize = item2.Value<long>("FileSize"),
						CreateDateTime = item2.Value<DateTime>("CreateDateTime"),
						NeedTakesTime = item2.Value<string>("NeedTakesTime"),
						UnZip = item2.Value<bool>("UnZip"),
						Status = (DownloadStatus)Enum.Parse(typeof(DownloadStatus), item2.Value<string>("Status")),
						ZipPwd = item2.Value<string>("ZipPwd")
					};
					list.Add(item);
				}
			}
			GlobalFun.TryDeleteFile(path);
		}
		return list;
	}
}
