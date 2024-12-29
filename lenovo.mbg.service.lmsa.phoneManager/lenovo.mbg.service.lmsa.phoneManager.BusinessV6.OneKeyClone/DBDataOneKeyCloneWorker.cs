using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;

public class DBDataOneKeyCloneWorker : OnkeyCloneWorkerAbstract
{
	public DBDataOneKeyCloneWorker(TcpAndroidDevice device, string resourceType, int remoteServiceCode)
		: base(device, resourceType, remoteServiceCode)
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
