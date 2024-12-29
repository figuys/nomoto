using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class FileTransfer : IFileTransfer, IDisposable
{
	private SocketWrapper _socketWrapper;

	private FileReader _fileReader;

	protected RsaSocketDataSecurityFactory rsaSocketDataSecurityFactory;

	private volatile bool mIsDisposed;

	public bool IsDisposed
	{
		get
		{
			return mIsDisposed;
		}
		set
		{
			mIsDisposed = value;
		}
	}

	public FileTransfer(SocketWrapper socketWrapper, RsaSocketDataSecurityFactory encryptHelper)
	{
		_socketWrapper = socketWrapper;
		rsaSocketDataSecurityFactory = encryptHelper;
		_fileReader = new FileReader(socketWrapper, encryptHelper);
	}

	private TransferFileInfo CreateTransferFileInifo(List<PropItem> fileInfo)
	{
		TransferFileInfo transferFileInfo = new TransferFileInfo();
		PropInfo propInfo = new PropInfo();
		propInfo.AddOrUpdateProp(fileInfo);
		transferFileInfo.LogicFileName = propInfo.GetProp("filename");
		transferFileInfo.FilePath = propInfo.GetProp("path");
		transferFileInfo.ModifiedDateTime = propInfo.GetDateTimeProp("modifiedDateTime");
		transferFileInfo.FileSize = propInfo.GetLongProp("fileSize");
		return transferFileInfo;
	}

	public bool ReceiveFile(string localStorageDir, out long sequence, out TransferFileInfo transferFileInfo)
	{
		sequence = -1L;
		transferFileInfo = ReceiveFileInfo(out sequence);
		if (transferFileInfo == null)
		{
			return false;
		}
		return InternalReceiveFile(localStorageDir, transferFileInfo);
	}

	public bool SendFile(string locaFileFullName, long sequence)
	{
		return SendFile(locaFileFullName, locaFileFullName, sequence);
	}

	public bool SendFileInfo(TransferFileInfo transferFileInfo, long sequence)
	{
		try
		{
			return new FileWriter(_socketWrapper).WriteFileInfo(transferFileInfo, sequence, rsaSocketDataSecurityFactory);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool SendFile(string locaFileFullName, string targetFileFullName, long sequence)
	{
		LogHelper.LogInstance.Debug($"Begin send file:[LoaclFile:{locaFileFullName}][TargetFile:{targetFileFullName}]");
		try
		{
			FileInfo fileInfo = new FileInfo(locaFileFullName);
			using (FileStream fileStream = fileInfo.OpenRead())
			{
				string fileName = Path.GetFileName(targetFileFullName);
				TransferFileInfo transferFileInfo = new TransferFileInfo();
				transferFileInfo.LogicFileName = fileName;
				transferFileInfo.FilePath = targetFileFullName;
				transferFileInfo.ModifiedDateTime = fileInfo.LastWriteTime;
				transferFileInfo.FileSize = fileInfo.Length;
				FileWriter fileWriter = new FileWriter(_socketWrapper);
				if (fileWriter.WriteFileInfo(transferFileInfo, sequence, rsaSocketDataSecurityFactory))
				{
					int num = 1024;
					byte[] buffer = new byte[num];
					int num2 = 0;
					long num3 = 0L;
					while ((num2 = fileStream.Read(buffer, 0, num)) != 0 && fileWriter.CanWrite)
					{
						num3 += num2;
						fileWriter.WriteFileStream(buffer, 0, num2);
					}
					bool flag = num3 == transferFileInfo.FileSize;
					LogHelper.LogInstance.Debug($"Send file[LocalFile:{locaFileFullName}][TargetFile:{targetFileFullName}] finished, result {flag}");
					return flag;
				}
			}
			LogHelper.LogInstance.Debug($"Send file[{locaFileFullName}] failed");
			return false;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error($"Send file[{locaFileFullName}] failed, throw exception:{ex.ToString()}");
			throw ex;
		}
	}

	public bool SendBytes(byte[] content)
	{
		try
		{
			FileWriter fileWriter = new FileWriter(_socketWrapper);
			int num = content.Length;
			return fileWriter.WriteFileStream(content, 0, num) == num;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void Dispose()
	{
		if (!IsDisposed)
		{
			IsDisposed = true;
			if (_socketWrapper != null)
			{
				_socketWrapper.Dispose();
			}
			if (_fileReader != null)
			{
				_fileReader.Dispose();
			}
		}
	}

	public TransferFileInfo ReceiveFileInfo(out long sequence)
	{
		try
		{
			FileReader fileReader = _fileReader;
			List<PropItem> remoteFileInfo = null;
			LogHelper.LogInstance.Info("Begin receive transfer file info");
			if (fileReader.ReciveFileInfo(60000, out remoteFileInfo, out sequence) && remoteFileInfo != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (PropItem item in remoteFileInfo)
				{
					stringBuilder.Append($"{item.Key}:{item.Value}{Environment.NewLine}");
				}
				LogHelper.LogInstance.Debug("Received remote file info:" + stringBuilder.ToString());
				TransferFileInfo result = CreateTransferFileInifo(remoteFileInfo);
				LogHelper.LogInstance.Info("Receive transfer file info suceeded");
				return result;
			}
			LogHelper.LogInstance.Info("Receive transfer file info failed");
			return null;
		}
		catch (SocketException ex)
		{
			LogHelper.LogInstance.Error("Receive file info throw socketException:" + ex.ToString());
			throw;
		}
		catch (Exception ex2)
		{
			LogHelper.LogInstance.Error("Receive file info throw exception:" + ex2.ToString());
			throw;
		}
	}

	private bool InternalReceiveFile(string localStorageDir, TransferFileInfo transferFileInfo)
	{
		FileReader fileReader = _fileReader;
		try
		{
			string text = Path.Combine(localStorageDir, transferFileInfo.VirtualFileName);
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			else
			{
				string directoryName = Path.GetDirectoryName(text);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
			}
			transferFileInfo.localFilePath = text;
			long fileSize = transferFileInfo.FileSize;
			byte[] buffer = new byte[1024];
			long num = 0L;
			int num2 = 0;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				while (num < fileSize && fileReader.CanReadFileStream)
				{
					num2 = fileReader.ReadFileStream(ref buffer, 0, 1024);
					if (num2 == 0)
					{
						break;
					}
					num += num2;
					memoryStream.Write(buffer, 0, num2);
				}
				if (num != fileSize)
				{
					LogHelper.LogInstance.Debug($"Receive file[{transferFileInfo.FilePath}] finished,result false");
					return false;
				}
				using FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write);
				byte[] array = rsaSocketDataSecurityFactory.Decrypt(memoryStream.ToArray());
				fileStream.Write(array, 0, array.Length);
				fileStream.Flush();
			}
			LogHelper.LogInstance.Debug($"Receive file[{transferFileInfo.FilePath}] finished,result true");
			return true;
		}
		catch (SocketException ex)
		{
			LogHelper.LogInstance.Error("Connect data server throw socketException:" + ex.ToString());
			throw ex;
		}
		catch (Exception ex2)
		{
			LogHelper.LogInstance.Error("Connect data server throw exception:" + ex2.ToString());
			throw ex2;
		}
	}
}
