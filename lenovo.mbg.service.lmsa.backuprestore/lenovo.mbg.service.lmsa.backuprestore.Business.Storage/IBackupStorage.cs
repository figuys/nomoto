using System;
using System.IO;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public interface IBackupStorage : IDisposable
{
	string StoragePath { get; }

	long Position { get; }

	long Size { get; }

	void Seek(long offset, SeekOrigin seekOrigin);

	void SetLength(long length);

	IBackupResourceWriter OpenWrite(string version);

	void Write(byte[] buffer, int offset, int count);

	IBackupResourceReader OpenRead(out string version);

	int Read(byte[] buffer, int offset, int count);

	void Flush();

	void Delete();
}
