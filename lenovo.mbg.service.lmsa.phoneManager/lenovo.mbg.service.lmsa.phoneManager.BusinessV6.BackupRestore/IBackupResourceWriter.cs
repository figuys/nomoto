using System;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public interface IBackupResourceWriter : IDisposable
{
	bool CanWrite();

	void SetPassword(string password);

	void BeginWrite();

	void ReserveDiskSpace(int resourceItemsCount, long reservereSourceStreamGross);

	IBackupResourceStreamWriter Seek(BackupResource resource);

	void Write(BackupResource resource);

	void RemoveEnd();

	void EndWrite();
}
