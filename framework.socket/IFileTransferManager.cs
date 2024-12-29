using System;

namespace lenovo.mbg.service.framework.socket;

public interface IFileTransferManager : IDisposable
{
	FileTransferWrapper CreateFileTransfer(long messageSequence);
}
