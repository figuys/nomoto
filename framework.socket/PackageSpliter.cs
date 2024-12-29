using System;
using System.Collections.Concurrent;

namespace lenovo.mbg.service.framework.socket;

public class PackageSpliter
{
	private bool _removePackageSpliter;

	protected readonly byte[] _packageSplitBytes;

	private ConcurrentQueue<byte[]> _cache;

	private const int EX_BUFFER_SIZE = 1024;

	private int _writeBufferSize = 1024;

	private byte[] _writeBuffer;

	private int _writeOffset;

	private byte[] _splitResultCache;

	public int Available { get; private set; }

	public PackageSpliter(byte[] packageSplitBytes, bool removePackageSpliter)
	{
		_packageSplitBytes = packageSplitBytes;
		_removePackageSpliter = removePackageSpliter;
		_cache = new ConcurrentQueue<byte[]>();
		ResetToDefault();
	}

	private void ResetToDefault()
	{
		_writeBufferSize = 1024;
		_writeBuffer = new byte[_writeBufferSize];
		_writeOffset = 0;
	}

	public void Write(byte[] buffer, int offset, int size)
	{
		if (ResetBufferSize(size))
		{
			ResetBufferCapacity();
		}
		Array.Copy(buffer, 0, _writeBuffer, _writeOffset, size);
		_writeOffset += size;
		DoSplit();
	}

	private bool ResetBufferSize(int size)
	{
		bool result = false;
		int num = ((_writeBufferSize - _writeOffset > size) ? size : (_writeBufferSize - _writeOffset));
		while (num < size)
		{
			_writeBufferSize += 1024;
			num = ((_writeBufferSize - _writeOffset > size) ? size : (_writeBufferSize - _writeOffset));
			result = true;
		}
		return result;
	}

	private void ResetBufferCapacity()
	{
		byte[] writeBuffer = _writeBuffer;
		_writeBuffer = new byte[_writeBufferSize];
		Array.Copy(writeBuffer, 0, _writeBuffer, 0, _writeOffset);
	}

	private void Defragment(int offset)
	{
		int num = _writeOffset - offset;
		for (int i = 0; i < num; i++)
		{
			_writeBuffer[i] = _writeBuffer[i + offset];
		}
		_writeOffset = num;
	}

	private void DoSplit()
	{
		byte[] spliteBuffer = null;
		if (Split(out spliteBuffer))
		{
			Available = 1;
			_splitResultCache = spliteBuffer;
		}
		else
		{
			Available = 0;
		}
	}

	private bool Split(out byte[] spliteBuffer)
	{
		spliteBuffer = null;
		byte[] packageSplitBytes = _packageSplitBytes;
		int num = packageSplitBytes.Length;
		int writeOffset = _writeOffset;
		bool result = false;
		byte[] writeBuffer = _writeBuffer;
		for (int i = 0; i < writeOffset; i++)
		{
			if (writeBuffer[i] != packageSplitBytes[0] || i > writeOffset - num)
			{
				continue;
			}
			int j;
			for (j = 0; j < num && writeBuffer[i + j] == packageSplitBytes[j]; j++)
			{
			}
			if (j >= num)
			{
				result = true;
				int num2 = i + j;
				int num3 = (_removePackageSpliter ? i : num2);
				spliteBuffer = new byte[num3];
				Array.Copy(_writeBuffer, 0, spliteBuffer, 0, num3);
				if (writeOffset - num2 > 0)
				{
					Defragment(num2);
				}
				else
				{
					ResetToDefault();
				}
				OnSplited(spliteBuffer);
				break;
			}
		}
		return result;
	}

	protected virtual void OnSplited(byte[] bytes)
	{
	}

	public bool Read(out byte[] buffer)
	{
		if (Available > 0)
		{
			int num = _splitResultCache.Length;
			buffer = new byte[num];
			Array.Copy(_splitResultCache, 0, buffer, 0, num);
			_splitResultCache = null;
			DoSplit();
			return true;
		}
		buffer = null;
		return false;
	}

	public bool ReadAll(out byte[] buffer)
	{
		int num = 0;
		if (Available > 0)
		{
			num = _splitResultCache.Length;
		}
		int num2 = num + _writeOffset;
		if (num2 == 0)
		{
			buffer = new byte[0];
			return true;
		}
		byte[] array = new byte[num2];
		if (num > 0)
		{
			Array.Copy(_splitResultCache, 0, array, 0, num);
		}
		if (_writeOffset > 0)
		{
			Array.Copy(_writeBuffer, 0, array, num, _writeOffset);
		}
		buffer = array;
		return true;
	}
}
