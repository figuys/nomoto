using System.Collections.Generic;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public class CompareDictionary : IEqualityComparer<AbstractDownloadInfo>
{
	bool IEqualityComparer<AbstractDownloadInfo>.Equals(AbstractDownloadInfo x, AbstractDownloadInfo y)
	{
		if (x.downloadFileName.Equals(y.downloadFileName) && x.downloadMD5.Equals(y.downloadMD5))
		{
			return x.saveLocalPath.Equals(y.saveLocalPath);
		}
		return false;
	}

	int IEqualityComparer<AbstractDownloadInfo>.GetHashCode(AbstractDownloadInfo obj)
	{
		return obj.ToString().GetHashCode();
	}
}
