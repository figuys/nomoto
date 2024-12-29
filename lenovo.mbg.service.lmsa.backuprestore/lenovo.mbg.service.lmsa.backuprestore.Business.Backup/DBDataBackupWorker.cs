using System.Collections.Generic;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Backup;

public class DBDataBackupWorker : BackupWorkerAbstractEx
{
	public DBDataBackupWorker(TcpAndroidDevice device, IBackupResourceWriter backupResourceWriter, string resourceType, Dictionary<string, long> idAndSizeMapping, int remoteServiceCode)
		: base(device, backupResourceWriter, resourceType, idAndSizeMapping, remoteServiceCode)
	{
		base.GetPathByIdWhenPathIsEmpty = false;
	}

	protected override BackupResource CreateResourceHeader(BackupResource parent, string id, Header header)
	{
		return new BackupResource
		{
			ParentId = parent.Id,
			Value = "1",
			Tag = "file",
			AssociatedStreamSize = header.GetInt64("StreamLength", 0L)
		};
	}

	protected override string CreateResourceName(string id, string path)
	{
		return string.Empty;
	}
}
