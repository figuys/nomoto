using System.Collections.Generic;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadSave;

public interface ISaveDownloadInfoMode
{
	void SaveAbstractDownloadInfo(object obj);

	IEnumerable<AbstractDownloadInfo> GetAbstractDownloadInfoList();
}
