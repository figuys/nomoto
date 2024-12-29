using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public interface IBackupResourceReader : IDisposable
{
	bool CanRead();

	bool IsSetPassword();

	bool CheckPassword(string password);

	IBackupResourceStreamReader Seek(BackupResource resource);

	List<BackupResource> GetChildResources(BackupResource parent);

	void Foreach(BackupResource resource, Action<BackupResource> callback);
}
