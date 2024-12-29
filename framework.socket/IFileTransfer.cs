namespace lenovo.mbg.service.framework.socket;

public interface IFileTransfer
{
	bool SendFileInfo(TransferFileInfo transferFileInfo, long sequence);

	TransferFileInfo ReceiveFileInfo(out long sequence);

	bool ReceiveFile(string localStorageDir, out long sequence, out TransferFileInfo transferFileInfo);

	bool SendFile(string locaFilePath, long sequence);

	bool SendFile(string locaFileFullName, string targetFileFullName, long sequence);

	bool SendBytes(byte[] content);
}
