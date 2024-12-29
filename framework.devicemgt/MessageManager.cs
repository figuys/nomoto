using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

public class MessageManager : IMessageManager, IDisposable
{
	private IPEndPointInfo _remoteEndpoint;

	private SocketWrapper _heartbeatChannel;

	private bool _isDisposed;

	private ConcurrentBag<MessageReaderAndWriter> messageReaderAndWriterList = new ConcurrentBag<MessageReaderAndWriter>();

	public RsaSocketDataSecurityFactory RsaSocketEncryptHelper { get; private set; }

	public event EventHandler<HeartbeatStoppedEventArgs> HeartbeatStopped;

	public SocketWrapper GetHeartbeatChannel()
	{
		return _heartbeatChannel;
	}

	public MessageManager(IPEndPointInfo remoteEndPoint, RsaSocketDataSecurityFactory encryptHelper)
	{
		_remoteEndpoint = remoteEndPoint;
		RsaSocketEncryptHelper = encryptHelper;
	}

	public void StartHeartbeat(long sendHeartbeatInterval = 1000L, long receiveHeartbeatTimeout = 10000L)
	{
		HeartbeatSocketWrapper heartbeatSocketWrapper = new HeartbeatSocketWrapper(_remoteEndpoint, noDelay: true, RsaSocketEncryptHelper);
		heartbeatSocketWrapper.SetSendHeartbeatInterval(sendHeartbeatInterval);
		MessageReaderAndWriter messageReaderAndWriter = new MessageReaderAndWriter(new MessageWriter(heartbeatSocketWrapper, appendSpliterString: false, RsaSocketEncryptHelper), new MessageReader(heartbeatSocketWrapper, new PackageSpliter(Constants.Encoding.GetBytes("\\r\\n"), removePackageSpliter: true), RsaSocketEncryptHelper));
		List<PropItem> receiveData = null;
		messageReaderAndWriter.SendAndReceiveSync<object, PropItem>("startHeartbeat", "startHeartbeatResponse", null, Sequence.SingleInstance.New(), out receiveData);
		heartbeatSocketWrapper.StartSendHeartbeat();
		heartbeatSocketWrapper.StartReceiveHeartbeat();
		heartbeatSocketWrapper.HeartbeatStopped += delegate(object s, HeartbeatStoppedEventArgs e)
		{
			FireHeartbeatStoppedEvent(s, e);
		};
		_heartbeatChannel = heartbeatSocketWrapper;
	}

	private void FireHeartbeatStoppedEvent(object sender, HeartbeatStoppedEventArgs e)
	{
		if (this.HeartbeatStopped != null)
		{
			Delegate[] invocationList = this.HeartbeatStopped.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<HeartbeatStoppedEventArgs>)invocationList[i]).BeginInvoke(sender, e, null, null);
			}
		}
	}

	public MessageReaderAndWriter CreateMessageReaderAndWriter(int timeout = 0)
	{
		if (_isDisposed)
		{
			return null;
		}
		SocketWrapper socketWrapper = null;
		try
		{
			socketWrapper = new SocketWrapper(_remoteEndpoint, noDelay: true, isHeartbeatChannel: false, timeout);
			MessageReaderAndWriter messageReaderAndWriter = new MessageReaderAndWriter(new MessageWriter(socketWrapper, appendSpliterString: false, RsaSocketEncryptHelper), new MessageReader(socketWrapper, new PackageSpliter(Constants.Encoding.GetBytes("\\r\\n"), removePackageSpliter: true), RsaSocketEncryptHelper));
			CacheMessageReaderAndWriter(messageReaderAndWriter);
			return messageReaderAndWriter;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Warn("Create message changel failed:" + ex);
			if (socketWrapper != null)
			{
				try
				{
					socketWrapper.Dispose();
					socketWrapper = null;
				}
				catch
				{
				}
			}
			return null;
		}
	}

	public void Dispose()
	{
		if (_isDisposed)
		{
			return;
		}
		try
		{
			_heartbeatChannel?.Dispose();
			ReleaseMessageReaderAndWriter();
		}
		catch
		{
		}
	}

	private void CacheMessageReaderAndWriter(MessageReaderAndWriter messageReaderAndWriter)
	{
		messageReaderAndWriterList.Add(messageReaderAndWriter);
	}

	private void ReleaseMessageReaderAndWriter()
	{
		MessageReaderAndWriter result = null;
		while (!messageReaderAndWriterList.IsEmpty)
		{
			if (messageReaderAndWriterList.TryTake(out result))
			{
				((IDisposable)result)?.Dispose();
			}
		}
	}
}
