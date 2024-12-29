using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class FileTransferWrapper : IDisposable
{
	private SocketWrapper _channel;

	private IPEndPointInfo _remoteEndPoint;

	private RsaSocketDataSecurityFactory _RsaSocketEncryptHelper;

	private AutoResetEvent mTimeoutDoneResetEvent;

	private volatile bool mIsDisposed;

	private ConcurrentBag<IFileTransfer> fileTransfers = new ConcurrentBag<IFileTransfer>();

	public SocketWrapper Channel => _channel;

	public virtual bool IsDisposed
	{
		get
		{
			return mIsDisposed;
		}
		protected set
		{
			mIsDisposed = value;
		}
	}

	public FileTransferWrapper(IPEndPointInfo remoteEndPoint, RsaSocketDataSecurityFactory encryptHelper)
	{
		_remoteEndPoint = remoteEndPoint;
		_RsaSocketEncryptHelper = encryptHelper;
		_channel = new SocketWrapper(remoteEndPoint, noDelay: false);
	}

	public IPEndPointInfo GetIPEndPointInfo()
	{
		return _remoteEndPoint;
	}

	public virtual void NotifyFileReceiveComplete()
	{
		try
		{
			SocketWrapper channel = _channel;
			byte[] bytes = BitConverter.GetBytes(100);
			channel.Write(bytes);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Send file response[100] throw exception:" + ex.ToString());
			throw;
		}
	}

	public virtual bool WaitFileReceiveCompleteNotify(int timeout)
	{
		bool success = false;
		Exception taskExecption = null;
		AutoResetEvent done = new AutoResetEvent(initialState: false);
		try
		{
			mTimeoutDoneResetEvent = done;
			Action _ac = delegate
			{
				success = WaitFileReceiveCompleteNotify();
			};
			_ac.BeginInvoke(delegate(IAsyncResult ar)
			{
				try
				{
					_ac.EndInvoke(ar);
				}
				catch (Exception ex)
				{
					success = false;
					taskExecption = ex;
				}
				try
				{
					done.Set();
				}
				catch (Exception)
				{
				}
			}, null);
			if (!done.WaitOne(timeout))
			{
				success = false;
				throw new TimeoutException("wait file receive complete notify timeout", taskExecption);
			}
			if (taskExecption != null)
			{
				throw new Exception("wait file receive complete notify throw exception, see inner exception", taskExecption);
			}
			if (IsDisposed)
			{
				success = false;
			}
		}
		finally
		{
			if (done != null)
			{
				((IDisposable)done).Dispose();
			}
		}
		return success;
	}

	private bool WaitFileReceiveCompleteNotify()
	{
		SocketWrapper channel = _channel;
		int num = 4;
		byte[] buffer = new byte[num];
		while (channel.Connected)
		{
			try
			{
				if (channel.Read(ref buffer, 0, num, SocketFlags.None) != 0)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Receive file response[100] throw exception:" + ex.ToString());
				throw;
			}
			Thread.Sleep(1000);
		}
		return false;
	}

	public virtual bool ReceiveFile(string localStorageDir, out TransferFileInfo transferFileInfo, out long sequence)
	{
		return InternalReceiveFile(localStorageDir, out transferFileInfo, out sequence);
	}

	public virtual bool ReceiveFile(string localStorageDir, out TransferFileInfo transferFileInfo)
	{
		long sequence = -1L;
		return InternalReceiveFile(localStorageDir, out transferFileInfo, out sequence);
	}

	public virtual bool SendFile(string locaFileFullName, long sequence)
	{
		return SendFile(locaFileFullName, locaFileFullName, sequence);
	}

	public virtual bool SendFile(string locaFileFullName, string targetFileFullName, long sequence)
	{
		return InternalSendFile(locaFileFullName, targetFileFullName, sequence);
	}

	protected virtual bool InternalReceiveFile(string localStorageDir, out TransferFileInfo transferFileInfo, out long sequence)
	{
		IFileTransfer fileTransfer = new FileTransfer(_channel, _RsaSocketEncryptHelper);
		CacheFileTransfer(fileTransfer);
		transferFileInfo = null;
		sequence = -1L;
		return fileTransfer.ReceiveFile(localStorageDir, out sequence, out transferFileInfo);
	}

	protected virtual bool InternalSendFile(string locaFileFullName, string targetFileFullName, long sequence)
	{
		IFileTransfer fileTransfer = new FileTransfer(_channel, _RsaSocketEncryptHelper);
		CacheFileTransfer(fileTransfer);
		return fileTransfer.SendFile(locaFileFullName, targetFileFullName, sequence);
	}

	public virtual void Dispose()
	{
		mIsDisposed = true;
		if (_channel != null)
		{
			try
			{
				_channel.Dispose();
				_channel = null;
			}
			catch (Exception)
			{
			}
		}
		try
		{
			mTimeoutDoneResetEvent?.Set();
			mTimeoutDoneResetEvent?.SafeWaitHandle.Close();
			mTimeoutDoneResetEvent?.Close();
		}
		catch (Exception)
		{
		}
	}

	private void CacheFileTransfer(IFileTransfer transfer)
	{
		fileTransfers.Add(transfer);
	}

	private void Release()
	{
		IFileTransfer result = null;
		while (!fileTransfers.IsEmpty)
		{
			if (fileTransfers.TryTake(out result) && result != null)
			{
				((IDisposable)result).Dispose();
			}
		}
	}
}
