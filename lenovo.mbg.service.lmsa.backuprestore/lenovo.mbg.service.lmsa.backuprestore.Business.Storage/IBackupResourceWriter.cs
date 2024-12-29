using System;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

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
