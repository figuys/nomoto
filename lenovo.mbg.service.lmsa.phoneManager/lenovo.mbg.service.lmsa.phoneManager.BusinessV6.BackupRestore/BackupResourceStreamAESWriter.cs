using System;
using System.IO;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupResourceStreamAESWriter : IBackupResourceStreamWriter
{
	private IBackupStorage _storage;

	private long _storageOffset;

	private long _plaintextLength;

	private AES _aes;

	private byte[] _plaintextBuffer;

	private int _plaintextBufferOffset = 4;

	public BackupResourceStreamAESWriter(IBackupStorage storage, AES aes, long storageOffset, long plaintextLength)
	{
		if (plaintextLength != 0L)
		{
			_storage = storage;
			_storageOffset = storageOffset;
			_plaintextLength = plaintextLength;
			storage.Seek(storageOffset, SeekOrigin.Begin);
			_plaintextBuffer = new byte[1600];
			_aes = aes;
		}
	}

	public void BeginWrite()
	{
	}

	public void Write(byte[] buffer, int offset, int count, long sourceCount)
	{
		if (_plaintextLength == 0L || count == 0)
		{
			return;
		}
		int num = count;
		int num2 = 1600 - _plaintextBufferOffset;
		int num3 = ((num2 > num) ? num : num2);
		do
		{
			Array.Copy(buffer, offset, _plaintextBuffer, _plaintextBufferOffset, num3);
			offset += num3;
			_plaintextBufferOffset += num3;
			num -= num3;
			num2 = 1600 - _plaintextBufferOffset;
			if (num2 == 0)
			{
				InternalWrite(_plaintextBuffer);
				_plaintextBufferOffset = 4;
				num2 = 1600 - _plaintextBufferOffset;
			}
			num3 = ((num2 > num) ? num : num2);
		}
		while (num != 0);
	}

	private void InternalWrite(byte[] buffer)
	{
		Array.Copy(BitConverter.GetBytes(_plaintextBufferOffset - 4), 0, _plaintextBuffer, 0, 4);
		byte[] array = _aes.Encrypt(_plaintextBuffer);
		_storage.Write(array, 0, array.Length);
	}

	public void EndWrite()
	{
		if (_plaintextLength != 0L && _plaintextBufferOffset > 4)
		{
			InternalWrite(_plaintextBuffer);
		}
	}

	public void Seek(long offset)
	{
	}
}
