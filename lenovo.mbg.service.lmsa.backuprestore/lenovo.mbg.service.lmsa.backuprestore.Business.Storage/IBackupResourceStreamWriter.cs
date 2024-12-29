namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public interface IBackupResourceStreamWriter
{
	void Seek(long offset);

	void BeginWrite();

	void Write(byte[] buffer, int offset, int count, long sourceCount);

	void EndWrite();
}
