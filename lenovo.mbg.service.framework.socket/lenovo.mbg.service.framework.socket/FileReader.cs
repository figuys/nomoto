using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class FileReader : IDisposable
{
	private SocketWrapperBase _socketWrapper;

	protected RsaSocketDataSecurityFactory rsaSocketDataSecurityFactory;

	private int _fileStreamCacheBufferLength;

	private byte[] _fileStreamCacheBuffer;

	private int _fileStreamCacheBufferReadOffset;

	private AutoResetEvent timeoutResetEvent;

	private PackageSpliter _spliter;

	private volatile bool mIsDisposed;

	public bool ReceivedFileInfo { get; protected set; }

	public bool CanReadFileStream
	{
		get
		{
			if (_socketWrapper != null && _socketWrapper.Connected)
			{
				return ReceivedFileInfo;
			}
			return false;
		}
	}

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

	public FileReader(SocketWrapperBase socketWrapper, RsaSocketDataSecurityFactory encryptHelper)
	{
		_socketWrapper = socketWrapper;
		rsaSocketDataSecurityFactory = encryptHelper;
		_spliter = new PackageSpliter(Constants.Encoding.GetBytes("\\r\\n"), removePackageSpliter: true);
	}

	protected virtual int ReadSocket(byte[] buffer, int offset, int size)
	{
		if (_socketWrapper == null || !_socketWrapper.Connected)
		{
			return -1;
		}
		return _socketWrapper.Read(ref buffer, offset, size, SocketFlags.None);
	}

	public bool ReciveFileInfo<T>(out List<T> remoteFileInfo, out long sequence)
	{
		MessageEx<T> msg = null;
		sequence = -1L;
		bool flag2 = (ReceivedFileInfo = ReadFileInfo(out msg));
		if (flag2 && msg != null && msg.Data != null)
		{
			if (!string.IsNullOrEmpty(msg.Sequence))
			{
				long.TryParse(msg.Sequence, out sequence);
			}
			remoteFileInfo = msg.Data;
			return true;
		}
		remoteFileInfo = null;
		return false;
	}

	public bool ReciveFileInfo<T>(int millisecondTimeout, out List<T> remoteFileInfo, out long sequence)
	{
		remoteFileInfo = null;
		sequence = -1L;
		Exception taskException = null;
		if (millisecondTimeout == -1)
		{
			return ReciveFileInfo(out remoteFileInfo, out sequence);
		}
		bool result = false;
		AutoResetEvent done = new AutoResetEvent(initialState: false);
		try
		{
			timeoutResetEvent = null;
			List<T> tempFileInfo = null;
			long temp_sequence = -1L;
			Action action = delegate
			{
				List<T> remoteFileInfo2 = null;
				ReciveFileInfo(out remoteFileInfo2, out temp_sequence);
				tempFileInfo = remoteFileInfo2;
				try
				{
					done.Set();
				}
				catch (Exception)
				{
				}
			};
			action.BeginInvoke(delegate(IAsyncResult ar)
			{
				try
				{
					action.EndInvoke(ar);
				}
				catch (Exception ex2)
				{
					taskException = ex2;
				}
			}, null);
			if (!done.WaitOne(millisecondTimeout))
			{
				if (taskException != null)
				{
					throw new Exception("Read file info throw exception,detail seee inner exception", taskException);
				}
				throw new TimeoutException("Read file info timeout", taskException);
			}
			if (!IsDisposed)
			{
				remoteFileInfo = tempFileInfo;
				sequence = temp_sequence;
				result = true;
			}
			else
			{
				result = false;
			}
		}
		finally
		{
			if (done != null)
			{
				((IDisposable)done).Dispose();
			}
		}
		return result;
	}

	private bool ReadFileInfo<T>(out MessageEx<T> msg)
	{
		msg = null;
		if (ReceivedFileInfo)
		{
			return true;
		}
		bool flag = false;
		int num = 1024;
		byte[] buffer = new byte[num];
		int num2 = 0;
		byte[] buffer2 = null;
		while (_socketWrapper != null && _socketWrapper.Connected)
		{
			try
			{
				while (_socketWrapper != null && _socketWrapper.Connected && !(flag = _spliter.Read(out buffer2)))
				{
					num2 = _socketWrapper.Read(ref buffer, 0, num, SocketFlags.None);
					if (num2 != 0 && num2 > 0)
					{
						_spliter.Write(buffer, 0, num2);
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Read file info throw exception:" + ex.ToString());
				flag = false;
				throw ex;
			}
			if (flag)
			{
				string @string = Constants.Encoding.GetString(buffer2);
				@string = rsaSocketDataSecurityFactory.DecryptFromBase64(@string);
				MessageEx<T> messageEx = JsonUtils.Parse<MessageEx<T>>(@string);
				if (messageEx != null)
				{
					msg = messageEx;
				}
				break;
			}
		}
		byte[] buffer3 = null;
		if (_spliter.ReadAll(out buffer3))
		{
			_fileStreamCacheBufferLength = buffer3.Length;
			_fileStreamCacheBufferReadOffset = 0;
			_fileStreamCacheBuffer = buffer3;
		}
		return flag;
	}

	public int ReadFileStream(ref byte[] buffer, int offset, int size)
	{
		if (!CanReadFileStream)
		{
			return 0;
		}
		if (_fileStreamCacheBufferLength - _fileStreamCacheBufferReadOffset > 0)
		{
			int num = ((_fileStreamCacheBufferLength - _fileStreamCacheBufferReadOffset > size) ? size : (_fileStreamCacheBufferLength - _fileStreamCacheBufferReadOffset));
			Array.Copy(_fileStreamCacheBuffer, _fileStreamCacheBufferReadOffset, buffer, 0, num);
			_fileStreamCacheBufferReadOffset += size;
			return num;
		}
		return _socketWrapper.Read(ref buffer, offset, size, SocketFlags.None);
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
		try
		{
			timeoutResetEvent.Set();
			timeoutResetEvent.Close();
		}
		catch (Exception)
		{
		}
	}
}
