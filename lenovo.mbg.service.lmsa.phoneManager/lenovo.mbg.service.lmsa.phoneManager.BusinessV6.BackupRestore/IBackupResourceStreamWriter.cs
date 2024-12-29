namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public interface IBackupResourceStreamWriter
{
	void Seek(long offset);

	void BeginWrite();

	void Write(byte[] buffer, int offset, int count, long sourceCount);

	void EndWrite();
}
