using System;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class HugeMemoryStream : Stream
{
	private const int PAGE_SIZE = 1024000;

	private const int ALLOC_STEP = 1024;

	private byte[][] _streamBuffers;

	private int _pageCount;

	private long _allocatedBytes;

	private long _position;

	private long _length;

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => true;

	public override long Length => _length;

	public override long Position
	{
		get
		{
			return _position;
		}
		set
		{
			if (value > _length)
			{
				throw new InvalidOperationException("Position > Length");
			}
			if (value < 0)
			{
				throw new InvalidOperationException("Position < 0");
			}
			_position = value;
		}
	}

	private int GetPageCount(long length)
	{
		int num = (int)(length / 1024000) + 1;
		if (length % 1024000 == 0L)
		{
			num--;
		}
		return num;
	}

	private void ExtendPages()
	{
		if (_streamBuffers == null)
		{
			_streamBuffers = new byte[1024][];
		}
		else
		{
			byte[][] array = new byte[_streamBuffers.Length + 1024][];
			Array.Copy(_streamBuffers, array, _streamBuffers.Length);
			_streamBuffers = array;
		}
		_pageCount = _streamBuffers.Length;
	}

	private void AllocSpaceIfNeeded(long value)
	{
		if (value < 0)
		{
			throw new InvalidOperationException("AllocSpaceIfNeeded < 0");
		}
		if (value == 0L)
		{
			return;
		}
		int pageCount = GetPageCount(_allocatedBytes);
		int pageCount2 = GetPageCount(value);
		while (pageCount < pageCount2)
		{
			if (pageCount == _pageCount)
			{
				ExtendPages();
			}
			_streamBuffers[pageCount++] = new byte[1024000];
		}
		_allocatedBytes = (long)pageCount * 1024000L;
		value = Math.Max(value, _length);
		if (_position > (_length = value))
		{
			_position = _length;
		}
	}

	public override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = (int)(_position / 1024000);
		int num2 = (int)(_position % 1024000);
		int num3 = 1024000 - num2;
		long position = _position;
		if (position + count > _length)
		{
			count = (int)(_length - position);
		}
		while (count != 0 && _position < _length)
		{
			if (num3 > count)
			{
				num3 = count;
			}
			Array.Copy(_streamBuffers[num++], num2, buffer, offset, num3);
			offset += num3;
			_position += num3;
			count -= num3;
			num2 = 0;
			num3 = 1024000;
		}
		return (int)(_position - position);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Current:
			offset += _position;
			break;
		case SeekOrigin.End:
			offset = _length - offset;
			break;
		default:
			throw new ArgumentOutOfRangeException("origin");
		case SeekOrigin.Begin:
			break;
		}
		return Position = offset;
	}

	public override void SetLength(long value)
	{
		if (value < 0)
		{
			throw new InvalidOperationException("SetLength < 0");
		}
		if (value == 0L)
		{
			_streamBuffers = null;
			_allocatedBytes = (_position = (_length = 0L));
			_pageCount = 0;
			return;
		}
		int num = GetPageCount(_allocatedBytes);
		int pageCount = GetPageCount(value);
		while (num > pageCount)
		{
			_streamBuffers[--num] = null;
		}
		AllocSpaceIfNeeded(value);
		if (_position > (_length = value))
		{
			_position = _length;
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		int num = (int)(_position / 1024000);
		int num2 = (int)(_position % 1024000);
		int num3 = 1024000 - num2;
		_ = _position;
		AllocSpaceIfNeeded(_position + count);
		while (count != 0)
		{
			if (num3 > count)
			{
				num3 = count;
			}
			Array.Copy(buffer, offset, _streamBuffers[num++], num2, num3);
			offset += num3;
			_position += num3;
			count -= num3;
			num2 = 0;
			num3 = 1024000;
		}
	}
}
