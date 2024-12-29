using System;
using System.Net.Sockets;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class HeartbeatSocketWrapper : SocketWrapper
{
	private volatile bool _sendingHeartbeat;

	private long _sendHeartbeatInterval = 1000L;

	private byte[] _sendHeartbeatPackage;

	private long _receivedHeartbeatTimeout = 3000L;

	private TimeoutClock _timeoutClock;

	public RsaSocketDataSecurityFactory _RsaSocketEncryptHelper;

	private volatile bool isHeartbeatStopped;

	private volatile bool isTimeout;

	private volatile bool _isManualClose;

	private Thread thread;

	public bool IsHeartbeatStopped
	{
		get
		{
			return isHeartbeatStopped;
		}
		private set
		{
			if (isHeartbeatStopped != value)
			{
				isHeartbeatStopped = value;
				if (isHeartbeatStopped)
				{
					FireHeartbeatStoppedEvent(this, new HeartbeatStoppedEventArgs(IsManualClose, IsTimeout));
				}
			}
		}
	}

	public bool IsTimeout
	{
		get
		{
			return isTimeout;
		}
		set
		{
			isTimeout = value;
		}
	}

	public new bool IsManualClose
	{
		get
		{
			return _isManualClose;
		}
		set
		{
			_isManualClose = value;
		}
	}

	private event EventHandler<HeartbeatStoppedEventArgs> heartbeatStopped;

	public event EventHandler<HeartbeatStoppedEventArgs> HeartbeatStopped
	{
		add
		{
			heartbeatStopped += value;
		}
		remove
		{
			heartbeatStopped -= value;
		}
	}

	public HeartbeatSocketWrapper(IPEndPointInfo remoteEndPoint, bool noDelay, RsaSocketDataSecurityFactory encryptHelper)
		: base(remoteEndPoint, noDelay, isHeartbeatChannel: true, 4000)
	{
		_RsaSocketEncryptHelper = encryptHelper;
	}

	private void FireHeartbeatStoppedEvent(object sender, HeartbeatStoppedEventArgs e)
	{
		if (this.heartbeatStopped != null)
		{
			Delegate[] invocationList = this.heartbeatStopped.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<HeartbeatStoppedEventArgs>)invocationList[i]).BeginInvoke(this, e, null, null);
			}
		}
	}

	private void TimeoutHandler()
	{
		LogHelper.LogInstance.Warn($"heartbeat {_receivedHeartbeatTimeout} milliseconds timeout");
		Connected = false;
		IsTimeout = true;
		Dispose();
	}

	public new void Close()
	{
		if (!IsHeartbeatStopped || Connected)
		{
			IsHeartbeatStopped = true;
			Connected = false;
			IsManualClose = true;
			StopSendHeartbeat();
			StopReceiveHeartbeat();
		}
	}

	public new void SetSendHeartbeatInterval(long milliseconds)
	{
		_sendHeartbeatInterval = milliseconds;
	}

	public new void StartSendHeartbeat()
	{
		IsHeartbeatStopped = false;
		IsManualClose = false;
		IsTimeout = false;
		PackageSpliter ps = new PackageSpliter(Constants.Encoding.GetBytes("\\r\\n"), removePackageSpliter: true);
		if (_sendingHeartbeat)
		{
			return;
		}
		thread = new Thread((ThreadStart)delegate
		{
			_sendingHeartbeat = true;
			string text = "{\"action\":\"\",\"params\":[]}";
			string s = _RsaSocketEncryptHelper.EncryptToBase64(text);
			byte[] bytes = Constants.Encoding.GetBytes(s);
			int num = bytes.Length;
			byte[] bytes2 = BitConverter.GetBytes(num);
			int num2 = bytes2.Length;
			byte[] array = new byte[num + num2];
			Array.Copy(bytes2, 0, array, 0, num2);
			Array.Copy(bytes, 0, array, num2, num);
			_sendHeartbeatPackage = array;
			byte[] buffer = new byte[1024];
			int num3 = 0;
			do
			{
				try
				{
					byte[] buffer2 = null;
					Thread.Sleep(50);
					Write(_sendHeartbeatPackage);
					num3 = base.RawSocket.Receive(buffer, 0, 1024, SocketFlags.None);
					if (num3 != 0)
					{
						_timeoutClock?.ResetStart();
						ps.Write(buffer, 0, num3);
						if (ps.Read(out buffer2))
						{
							string @string = Constants.Encoding.GetString(buffer2);
							@string = _RsaSocketEncryptHelper.DecryptFromBase64(@string);
							if (@string.Contains("disconnectTcp"))
							{
								IsManualClose = true;
								LogHelper.LogInstance.Info("Heartbeat stoped, received response:" + @string);
								new MessageWriter(this, appendSpliterString: false, _RsaSocketEncryptHelper).Write(new MessageEx<object>
								{
									Action = "disconnectTcpResponse",
									Sequence = "-1",
									Data = null
								});
								break;
							}
						}
					}
				}
				catch
				{
					Thread.Sleep(500);
				}
			}
			while (_sendingHeartbeat && Connected);
			Close();
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public new void StopSendHeartbeat()
	{
		_sendingHeartbeat = false;
		Connected = false;
	}

	public new void StartReceiveHeartbeat()
	{
		_timeoutClock = new TimeoutClock(_receivedHeartbeatTimeout, TimeoutHandler);
		_timeoutClock.StartClock();
	}

	public new void StopReceiveHeartbeat()
	{
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

	public override void Dispose()
	{
		if (!IsDisposed)
		{
			base.Dispose();
			Close();
		}
	}
}
