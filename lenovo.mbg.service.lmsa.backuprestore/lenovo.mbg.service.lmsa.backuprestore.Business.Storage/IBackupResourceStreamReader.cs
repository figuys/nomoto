namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public interface IBackupResourceStreamReader
{
	void Seek(long offset);

	int Read(byte[] buffer, int offset, int count);
}
