using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public interface IBackupResourceReader : IDisposable
{
	bool CanRead();

	bool IsSetPassword();

	bool CheckPassword(string password);

	IBackupResourceStreamReader Seek(BackupResource resource);

	List<BackupResource> GetChildResources(BackupResource parent);

	void Foreach(BackupResource resource, Action<BackupResource> callback);
}
