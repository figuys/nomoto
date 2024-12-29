using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.socket;

public class MessageReaderAndWriter : IDisposable
{
	private MessageWriter _wiriter;

	private MessageReader _reader;

	private ReaderWriterLockSlim readerWriterLocklim = new ReaderWriterLockSlim();

	private volatile bool mDisposed;

	public string InstanceID { get; private set; }

	public bool Disposed
	{
		get
		{
			return mDisposed;
		}
		set
		{
			mDisposed = value;
		}
	}

	public MessageReaderAndWriter(MessageWriter writer, MessageReader reader)
	{
		InstanceID = Guid.NewGuid().ToString("N");
		_wiriter = writer;
		_reader = reader;
	}

	public bool TryEnterLock(int millisecondsTimeout)
	{
		try
		{
			if (readerWriterLocklim == null)
			{
				return false;
			}
			return readerWriterLocklim.TryEnterWriteLock(millisecondsTimeout);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(InstanceID + ":Try to enter work lock throw exception:" + ex.ToString());
			return false;
		}
	}

	public void ExitLock()
	{
		try
		{
			readerWriterLocklim?.ExitWriteLock();
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(InstanceID + ":Exit enter work lock throw exception:" + ex.ToString());
		}
	}

	public bool SendAndReceiveSync<TRequest, TResponse>(string action, string responseAction, List<TRequest> parameter, long sequence, out List<TResponse> receiveData)
	{
		receiveData = null;
		if (!TryEnterLock(10000))
		{
			return false;
		}
		try
		{
			return SendAndReceive(action, responseAction, parameter, sequence, out receiveData);
		}
		finally
		{
			ExitLock();
		}
	}

	public bool SendAndReceiveSync<TRequest, TResponse>(string action, List<TRequest> parameter, long sequence, out List<TResponse> receiveData)
	{
		return SendAndReceiveSync(action, MessageConstant.getResponseAction(action), parameter, sequence, out receiveData);
	}

	public bool SendAndReceive<TRequest, TResponse>(string action, string responseAction, List<TRequest> parameter, long sequence, out List<TResponse> receiveData)
	{
		receiveData = null;
		if (InternalSend(action, parameter, sequence))
		{
			long secquence = -1L;
			return InternalReceive(responseAction, out secquence, out receiveData);
		}
		return false;
	}

	public MessageEx<int> SendAndReceive(string action, List<string> parameter, long sequence)
	{
		MessageEx<int> message = null;
		if (!TryEnterLock(10000))
		{
			return message;
		}
		try
		{
			if (_wiriter.Write(new MessageEx<string>
			{
				Action = action,
				Sequence = $"{sequence}",
				Data = parameter
			}))
			{
				try
				{
					_reader.Read(out message);
				}
				catch (SocketException exception)
				{
					LogHelper.LogInstance.Error("socket disconnected, receive '" + action + "Response' error", exception);
				}
				catch (Exception exception2)
				{
					LogHelper.LogInstance.Error("socket receive '" + action + "Response' error", exception2);
				}
			}
			return message;
		}
		finally
		{
			ExitLock();
		}
	}

	public bool SendAndReceive<TRequest, TResponse>(string action, List<TRequest> parameter, long sequence, out List<TResponse> receiveData)
	{
		return SendAndReceive(action, MessageConstant.getResponseAction(action), parameter, sequence, out receiveData);
	}

	public bool Send<T>(string action, List<T> parameter, long secquence)
	{
		return InternalSend(action, parameter, secquence);
	}

	public bool Receive<T>(string responseAction, out long secquence, out List<T> receiveData, int timeout = 15000)
	{
		long secquence2 = -1L;
		List<T> receiveData2 = null;
		bool result = DoWithTimeout(() => InternalReceive(responseAction, out secquence2, out receiveData2), timeout);
		receiveData = receiveData2;
		secquence = secquence2;
		return result;
	}

	public bool Receive<T>(string responseAction, out List<T> receiveData, int timeout = 15000)
	{
		long secquence = -1L;
		return Receive(responseAction, out secquence, out receiveData, timeout);
	}

	private bool DoWithTimeout(Func<bool> task, int timeOut)
	{
		IAsyncResult asyncResult = task.BeginInvoke(null, null);
		asyncResult.AsyncWaitHandle.WaitOne(timeOut);
		bool result = task.EndInvoke(asyncResult);
		asyncResult.AsyncWaitHandle.Close();
		return result;
	}

	private bool InternalSend<T>(string action, List<T> parameter, long secquence)
	{
		MessageWriter wiriter = _wiriter;
		bool result = false;
		if (wiriter.Write(new MessageEx<T>
		{
			Action = action,
			Sequence = secquence.ToString(),
			Data = parameter
		}))
		{
			result = true;
		}
		return result;
	}

	private bool InternalReceive<T>(string responseAction, out long secquence, out List<T> receiveData)
	{
		bool result = false;
		receiveData = null;
		MessageReader reader = _reader;
		MessageEx<T> message = null;
		secquence = -1L;
		try
		{
			if (reader != null && reader.Read(out message) && message != null && responseAction.Equals(message.Action))
			{
				secquence = message.LongSequence;
				result = true;
				receiveData = message.Data;
			}
		}
		catch (SocketException exception)
		{
			LogHelper.LogInstance.Error("socket disconnected, receive '" + responseAction + "' error", exception);
		}
		catch (Exception exception2)
		{
			LogHelper.LogInstance.Error("socket receive '" + responseAction + "' error", exception2);
		}
		return result;
	}

	public void Dispose()
	{
		Disposed = true;
		try
		{
			readerWriterLocklim?.Dispose();
			readerWriterLocklim = null;
			_wiriter?.Dispose();
			_wiriter = null;
			_reader?.Dispose();
			_reader = null;
		}
		catch (Exception)
		{
		}
	}
}
