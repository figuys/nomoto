using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace lenovo.mbg.service.framework.socket;

public class FileWriter : IDisposable
{
	private SocketWrapper _socketWrapper;

	public bool CanWrite
	{
		get
		{
			if (_socketWrapper != null)
			{
				return _socketWrapper.Connected;
			}
			return false;
		}
	}

	public bool IsDisposed { get; set; }

	public FileWriter(SocketWrapper socketWrapper)
	{
		_socketWrapper = socketWrapper;
	}

	public bool WriteFileInfo(TransferFileInfo fileInfo, long sequence, RsaSocketDataSecurityFactory encryptHelper)
	{
		List<PropItem> data = new List<PropItem>
		{
			new PropItem
			{
				Key = "filename",
				Value = fileInfo.LogicFileName
			},
			new PropItem
			{
				Key = "path",
				Value = fileInfo.FilePath
			},
			new PropItem
			{
				Key = "modifiedDateTime",
				Value = fileInfo.ModifiedDateTime.ToString()
			},
			new PropItem
			{
				Key = "fileSize",
				Value = fileInfo.FileSize.ToString()
			}
		};
		MessageEx<PropItem> messageEx = new MessageEx<PropItem>();
		messageEx.Action = "transferFileInfo";
		messageEx.Sequence = sequence.ToString();
		messageEx.Data = data;
		MessageWriter messageWriter = new MessageWriter(_socketWrapper, appendSpliterString: false, encryptHelper);
		if (!messageWriter.CanWrite)
		{
			return false;
		}
		return messageWriter.Write(messageEx);
	}

	public int WriteFileStream(byte[] buffer, int offset, int size)
	{
		if (_socketWrapper != null && _socketWrapper.Connected)
		{
			return _socketWrapper.Write(buffer, offset, size, SocketFlags.None);
		}
		return 0;
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
		}
	}
}
