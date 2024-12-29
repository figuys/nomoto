using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class SocketWrapper : SocketWrapperBase
{
	private volatile bool _connected = true;

	private volatile bool _sendingHeartbeat;

	private long _sendHeartbeatInterval = 1000L;

	private byte[] _sendHeartbeatPackage;

	private Timer _heartbeatSender;

	private volatile bool _handlingReceiveHeartbeat;

	private long _receivedHeartbeatTimeout = 10000L;

	private int _heartbeatPackageLength = 30;

	private PackageSpliter _packageSpliter;

	private TimeoutClock _timeoutClock;

	public bool isHeartbeatChannel;

	private volatile bool _connectionTimeout;

	private volatile bool _isManualClose;

	private ConcurrentQueue<byte[]> _cache;

	private int _prevReadOffset;

	private AutoResetEvent _cacheAutoRestEvent;

	public override bool Connected
	{
		get
		{
			if (_connected)
			{
				return base.Connected;
			}
			return false;
		}
	}

	public int HeartbeatPackageLength => _heartbeatPackageLength;

	public bool ConnectionTimeout
	{
		get
		{
			return _connectionTimeout;
		}
		private set
		{
			if (_connectionTimeout != value)
			{
				_connectionTimeout = value;
				if (_connectionTimeout)
				{
					FireTimeoutEvent(this, new EventArgs());
				}
			}
		}
	}

	public bool IsManualClose => _isManualClose;

	private event EventHandler timeoutEvnet;

	public event EventHandler Timeout
	{
		add
		{
			timeoutEvnet += value;
			if (ConnectionTimeout)
			{
				FireTimeoutEvent(this, new EventArgs());
			}
		}
		remove
		{
			timeoutEvnet -= value;
		}
	}

	public SocketWrapper(IPEndPointInfo remoteEndPoint, bool noDelay, bool isHeartbeatChannel = false, int timeout = 0)
		: base(remoteEndPoint, noDelay, timeout)
	{
		this.isHeartbeatChannel = isHeartbeatChannel;
	}

	private void FireTimeoutEvent(object sender, EventArgs e)
	{
		if (this.timeoutEvnet != null)
		{
			Delegate[] invocationList = this.timeoutEvnet.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler)invocationList[i]).BeginInvoke(this, e, null, null);
			}
		}
	}

	private void TimeoutHandler()
	{
		LogHelper.LogInstance.Error("Socket connection timeout");
		_connected = false;
		ConnectionTimeout = true;
		Dispose();
	}

	public new void Close()
	{
		_connected = false;
		_isManualClose = true;
		StopSendHeartbeat();
		StopReceiveHeartbeat();
		DisposeResetEvent();
	}

	public void SetSendHeartbeatInterval(long milliseconds)
	{
		_sendHeartbeatInterval = milliseconds;
	}

	public void StartSendHeartbeat()
	{
		if (!_sendingHeartbeat)
		{
			_sendingHeartbeat = true;
			string s = "{\"action\":\"\",\"params\":[]}";
			byte[] bytes = Constants.Encoding.GetBytes(s);
			int num = bytes.Length;
			byte[] bytes2 = BitConverter.GetBytes(num);
			int num2 = bytes2.Length;
			byte[] array = new byte[num + num2];
			Array.Copy(bytes2, 0, array, 0, num2);
			Array.Copy(bytes, 0, array, num2, num);
			_sendHeartbeatPackage = array;
			_heartbeatSender = new Timer(StartSendHeartbeatTask, null, 0L, _sendHeartbeatInterval);
		}
	}

	public void StopSendHeartbeat()
	{
		_sendingHeartbeat = false;
		_connected = false;
		try
		{
			if (_heartbeatSender != null)
			{
				LogHelper.LogInstance.Info("Stop received heartbeat data from proper channel");
				StackTrace stackTrace = new StackTrace();
				LogHelper.LogInstance.Info("Stop received heartbeat data from proper channel call stack:" + stackTrace.ToString());
				_heartbeatSender.Dispose();
				_heartbeatSender = null;
			}
		}
		catch (Exception)
		{
		}
	}

	public void StartReceiveHeartbeat()
	{
		if (!_handlingReceiveHeartbeat)
		{
			_handlingReceiveHeartbeat = true;
			_cacheAutoRestEvent = new AutoResetEvent(initialState: false);
			_packageSpliter = new PackageSpliter(Constants.Encoding.GetBytes("\\r\\n"), removePackageSpliter: false);
			_cache = new ConcurrentQueue<byte[]>();
			StartReceiveRawStream();
			_timeoutClock = new TimeoutClock(_receivedHeartbeatTimeout, TimeoutHandler);
			_timeoutClock.StartClock();
		}
	}

	public void SetReceiveHeartbeatTimeout(long milliseconds)
	{
		_receivedHeartbeatTimeout = milliseconds;
	}

	public void StopReceiveHeartbeat()
	{
		_handlingReceiveHeartbeat = false;
		try
		{
			if (_timeoutClock != null)
			{
				_timeoutClock.StopClock();
				_timeoutClock.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}

	private void StartReceiveRawStream()
	{
		Task.Factory.StartNew(delegate
		{
			if (isHeartbeatChannel)
			{
				ReadSocketForHeatbeart();
			}
			else
			{
				ReadSocket();
			}
		});
	}

	private void ReadSocket()
	{
		int num = 1024;
		byte[] buffer = new byte[num];
		int num2 = 0;
		bool flag = false;
		byte[] buffer2 = null;
		try
		{
			while (Connected)
			{
				while (Connected && !(flag = _packageSpliter.Read(out buffer2)))
				{
					num2 = base.Read(ref buffer, 0, num, SocketFlags.None);
					if (num2 > 0)
					{
						_timeoutClock?.ResetStart();
						_packageSpliter.Write(buffer, 0, num2);
					}
				}
				if (!flag)
				{
					continue;
				}
				if (buffer2.Length > _heartbeatPackageLength)
				{
					_cache.Enqueue(buffer2);
					try
					{
						_cacheAutoRestEvent.Set();
					}
					catch (Exception)
					{
					}
				}
				else
				{
					LogHelper.LogInstance.Info("Received heartbeat data");
				}
			}
		}
		catch (Exception ex2)
		{
			LogHelper.LogInstance.Error("Receive data throw exception,set flag to closed,the exception:" + ex2.ToString());
			_connected = false;
			ConnectionTimeout = true;
			Dispose();
		}
	}

	private void ReadSocketForHeatbeart()
	{
		int num = 1024;
		byte[] buffer = new byte[num];
		int num2 = 0;
		try
		{
			while (Connected)
			{
				LogHelper.LogInstance.Info("Received heartbeat data from proper channel begin");
				num2 = base.Read(ref buffer, 0, num, SocketFlags.None);
				if (num2 > 0)
				{
					_timeoutClock?.ResetStart();
				}
				LogHelper.LogInstance.Info("Received heartbeat data from proper channel end, read length:" + num2);
			}
			LogHelper.LogInstance.Info("Received heartbeat data from proper channel finish");
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Receive heartbeat data from proper channel throw exception,set flag to closed,the exception:" + ex.ToString());
			_connected = false;
			ConnectionTimeout = true;
			Dispose();
		}
	}

	public override int Read(ref byte[] buffer, int offset, int size, SocketFlags socketFlags)
	{
		if (_handlingReceiveHeartbeat)
		{
			return ReadFromCache(buffer, offset, size, socketFlags);
		}
		if (!Connected)
		{
			return 0;
		}
		return base.Read(ref buffer, offset, size, SocketFlags.None);
	}

	private int ReadFromCache(byte[] buffer, int offset, int size, SocketFlags socketFlags)
	{
		if (_cache.Count <= 0 && !_cacheAutoRestEvent.SafeWaitHandle.IsClosed)
		{
			_cacheAutoRestEvent.WaitOne();
		}
		int result = 0;
		byte[] result2 = null;
		int num = 0;
		if (_cache.TryPeek(out result2))
		{
			num = result2.Length - _prevReadOffset;
			if (num <= size)
			{
				Array.Copy(result2, _prevReadOffset, buffer, 0, num);
				if (_cache.TryDequeue(out result2))
				{
					_prevReadOffset = 0;
					result = num;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				Array.Copy(result2, _prevReadOffset, buffer, 0, size);
				_prevReadOffset += size;
				result = size;
			}
		}
		return result;
	}

	private void StartSendHeartbeatTask(object obj)
	{
		try
		{
			LogHelper.LogInstance.Error("Send heartbeat data begin");
			Write(_sendHeartbeatPackage);
			LogHelper.LogInstance.Error("Send heartbeat data end");
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Send heartbeat data throw exception:" + ex.ToString());
			_heartbeatSender?.Dispose();
			_heartbeatSender = null;
		}
	}

	public override void Dispose()
	{
		if (!IsDisposed)
		{
			base.Dispose();
			StopReceiveHeartbeat();
			DisposeResetEvent();
			StopSendHeartbeat();
			IsDisposed = true;
			_connected = false;
			_isManualClose = true;
		}
	}

	private void DisposeResetEvent()
	{
		try
		{
			_cacheAutoRestEvent?.Set();
			_cacheAutoRestEvent?.Dispose();
		}
		catch (Exception)
		{
		}
	}
}
