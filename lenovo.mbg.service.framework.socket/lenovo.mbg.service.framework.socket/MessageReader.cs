using System;
using System.Net.Sockets;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class MessageReader : IDisposable
{
	private SocketWrapper _socketWrapper;

	private PackageSpliter _spliter;

	protected RsaSocketDataSecurityFactory rsaSocketDataSecurityFactory;

	public bool CanRead
	{
		get
		{
			if (_socketWrapper != null)
			{
				return _socketWrapper.Connected;
			}
			return false;
		}
	}

	public MessageReader(SocketWrapper socketWrapper, PackageSpliter spliter, RsaSocketDataSecurityFactory encryptHelper)
	{
		_socketWrapper = socketWrapper;
		rsaSocketDataSecurityFactory = encryptHelper;
		_spliter = spliter;
	}

	public bool Read<T>(out MessageEx<T> message)
	{
		message = null;
		int num = 0;
		bool flag = false;
		byte[] buffer = null;
		byte[] buffer2 = new byte[1024];
		while (_socketWrapper != null && _socketWrapper.Connected)
		{
			try
			{
				while (_socketWrapper != null && _socketWrapper.Connected && !(flag = _spliter.Read(out buffer)))
				{
					num = _socketWrapper.Read(ref buffer2, 0, 1024, SocketFlags.None);
					if (num > 0)
					{
						_spliter.Write(buffer2, 0, num);
					}
				}
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode == SocketError.TimedOut || ex.ErrorCode == 10060 || ex.ErrorCode == 10054)
				{
					_socketWrapper.Close();
					throw ex;
				}
			}
			catch (Exception ex2)
			{
				throw ex2;
			}
			if (flag)
			{
				string @string = Constants.Encoding.GetString(buffer);
				@string = rsaSocketDataSecurityFactory.DecryptFromBase64(@string);
				message = JsonUtils.Parse<MessageEx<T>>(@string);
				if (message.Action == "getContactListByContactIdResponse" || message.Action == "getContactDetailExResponse")
				{
					LogHelper.LogInstance.Debug("socket receive action: " + message.Action);
				}
				flag = message != null;
				break;
			}
		}
		return flag;
	}

	public void Dispose()
	{
		if (_socketWrapper != null)
		{
			try
			{
				_socketWrapper.Dispose();
				_socketWrapper = null;
			}
			catch
			{
			}
		}
	}
}
