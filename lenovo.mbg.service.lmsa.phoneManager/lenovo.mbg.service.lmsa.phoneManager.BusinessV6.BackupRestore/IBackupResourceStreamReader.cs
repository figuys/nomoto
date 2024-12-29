namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public interface IBackupResourceStreamReader
{
	void Seek(long offset);

	int Read(byte[] buffer, int offset, int count);
}
