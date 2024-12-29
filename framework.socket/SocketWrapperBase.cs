using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace lenovo.mbg.service.framework.socket;

public abstract class SocketWrapperBase : IDisposable
{
	private volatile bool mIsConnected;

	public Socket RawSocket { get; protected set; }

	public IPEndPoint RemoteEndPoint { get; protected set; }

	public virtual bool Connected
	{
		get
		{
			if (mIsConnected)
			{
				return RawSocket.Connected;
			}
			return false;
		}
		protected set
		{
			mIsConnected = value;
		}
	}

	public int Available
	{
		get
		{
			if (RawSocket != null)
			{
				return RawSocket.Available;
			}
			return 0;
		}
	}

	public virtual bool IsDisposed { get; protected set; }

	public SocketWrapperBase(Socket rawSocket)
	{
		RawSocket = rawSocket;
		RemoteEndPoint = (IPEndPoint)rawSocket.RemoteEndPoint;
	}

	public SocketWrapperBase(IPEndPointInfo remoteEndPoint, bool noDelay, int timeout = 0)
	{
		IPEndPoint iPEndPoint = remoteEndPoint;
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
		{
			NoDelay = noDelay
		};
		try
		{
			IAsyncResult asyncResult = socket.BeginConnect(iPEndPoint, null, null);
			socket.ReceiveTimeout = timeout;
			Connected = asyncResult.AsyncWaitHandle.WaitOne(2000);
		}
		catch
		{
			Connected = false;
			throw;
		}
		RawSocket = socket;
		RemoteEndPoint = iPEndPoint;
	}

	public virtual void Close()
	{
		Connected = false;
		if (RawSocket != null && RawSocket.Connected)
		{
			RawSocket.Close();
		}
	}

	public virtual int Read(ref byte[] buffer, int offset, int size, SocketFlags socketFlags)
	{
		if (RawSocket == null)
		{
			return 0;
		}
		int num = RawSocket.Receive(buffer, offset, size, socketFlags);
		if (num == 0 && Available == 0)
		{
			Connected = false;
		}
		return num;
	}

	public virtual int Write(byte[] buffer, int offset, int size, SocketFlags socketFlags)
	{
		if (RawSocket == null || !Connected)
		{
			return 0;
		}
		int num = RawSocket.Send(buffer, offset, size, socketFlags);
		if (num == 0)
		{
			Thread.Sleep(1000);
		}
		return num;
	}

	public virtual int Write(byte[] buffer)
	{
		if (RawSocket == null || !Connected)
		{
			return 0;
		}
		return Write(buffer, 0, buffer.Length, SocketFlags.None);
	}

	public virtual void Dispose()
	{
		if (IsDisposed)
		{
			return;
		}
		try
		{
			if (RawSocket != null)
			{
				if (RawSocket.Connected)
				{
					RawSocket.Disconnect(reuseSocket: false);
				}
				RawSocket.Close();
				RawSocket.Dispose();
			}
		}
		catch (Exception)
		{
		}
	}
}
