using System.Collections.Generic;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadSave;

public class SaveDownloadInfo2Json : ISaveDownloadInfoMode
{
	private static ReaderWriterLockSlim jsonLocker = new ReaderWriterLockSlim();

	private string downloadsave2jsonPath { get; set; }

	public SaveDownloadInfo2Json(string path)
	{
		downloadsave2jsonPath = path;
	}

	public void SaveAbstractDownloadInfo(object obj)
	{
		try
		{
			jsonLocker.EnterWriteLock();
			JsonHelper.SerializeObject2File(downloadsave2jsonPath, obj);
		}
		finally
		{
			jsonLocker.ExitWriteLock();
		}
	}

	public IEnumerable<AbstractDownloadInfo> GetAbstractDownloadInfoList()
	{
		List<AbstractDownloadInfo> list = new List<AbstractDownloadInfo>();
		try
		{
			jsonLocker.EnterReadLock();
			return JsonHelper.DeserializeJson2ListFromFile<AbstractDownloadInfo>(downloadsave2jsonPath);
		}
		finally
		{
			jsonLocker.ExitReadLock();
		}
	}
}
