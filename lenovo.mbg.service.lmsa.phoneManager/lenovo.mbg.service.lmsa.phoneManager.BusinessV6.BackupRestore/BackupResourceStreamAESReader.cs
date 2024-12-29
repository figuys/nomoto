using System;
using System.IO;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupResourceStreamAESReader : Stream, IBackupResourceStreamReader
{
	private IBackupStorage _storage;

	private readonly long _baseOffset;

	private readonly long _plaintextLength;

	private long _notReadedLength;

	private readonly long _endPosition;

	private readonly long _encryptLength;

	private long _currentPosition;

	private byte[] _plaintextBuffer;

	private readonly int _plaintextBufferSize = 1600;

	private int _plaintextBufferOffset = 4;

	private int _plaintextBufferEndPosition;

	private AES _aes;

	private byte[] _encryptBlock;

	private readonly int _encryptBlockSize;

	private int _encryptReadedLength;

	private int _encryptBufferOffset;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length => _plaintextLength;

	public override long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public BackupResourceStreamAESReader(IBackupStorage storage, AES aes, long offset, long length)
	{
		_storage = storage;
		_baseOffset = offset;
		_plaintextLength = length;
		_storage.Seek(_baseOffset, SeekOrigin.Begin);
		_aes = aes;
		_encryptBlock = new byte[1616];
		_encryptBlockSize = _encryptBlock.Length;
		_encryptLength = AES.GetEncryptByteLength(length, partBlockForPlaintext: true);
		_currentPosition = _baseOffset;
		_endPosition = _baseOffset + _encryptLength;
		_notReadedLength = _encryptLength;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (_plaintextBufferOffset == 4 || _plaintextBufferOffset == _plaintextBufferSize)
		{
			_plaintextBuffer = null;
			_plaintextBufferOffset = 4;
			_plaintextBufferEndPosition = 0;
			int num = EncyptDataRead();
			if (num != _encryptBlockSize)
			{
				if (num == 0)
				{
					return 0;
				}
				throw new BackupRestoreException("Read encyption resource stream error");
			}
			_plaintextBuffer = _aes.Decrypt(_encryptBlock);
			if (_plaintextBuffer == null || _plaintextBuffer.Length != _plaintextBufferSize)
			{
				throw new BackupRestoreException("Decrypt resource stream error");
			}
			int num2 = BitConverter.ToInt32(_plaintextBuffer, 0);
			_plaintextBufferEndPosition = _plaintextBufferOffset + num2;
		}
		count = ((_plaintextBufferEndPosition - _plaintextBufferOffset > count) ? count : (_plaintextBufferEndPosition - _plaintextBufferOffset));
		Array.Copy(_plaintextBuffer, _plaintextBufferOffset, buffer, offset, count);
		_plaintextBufferOffset += count;
		return count;
	}

	private int EncyptDataRead()
	{
		if (_currentPosition == _endPosition)
		{
			return 0;
		}
		if (_currentPosition < _baseOffset || _currentPosition > _endPosition)
		{
			throw new BackupRestoreException("Read pointer error!");
		}
		_encryptReadedLength = 0;
		_encryptBufferOffset = 0;
		do
		{
			_encryptReadedLength = _storage.Read(_encryptBlock, _encryptBufferOffset, _encryptBlockSize - _encryptBufferOffset);
			_currentPosition = _storage.Position;
			_encryptBufferOffset += _encryptReadedLength;
		}
		while (_encryptBufferOffset != _encryptBlockSize);
		return _encryptBlockSize;
	}

	public void Seek(long offset)
	{
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}
}
