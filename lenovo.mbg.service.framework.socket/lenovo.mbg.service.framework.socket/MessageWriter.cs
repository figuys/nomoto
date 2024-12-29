using System;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public class MessageWriter : IDisposable
{
	private SocketWrapper _socketWrapper;

	private bool _appendSpliterString;

	public RsaSocketDataSecurityFactory rsaSocketDataSecurityFactory;

	public bool CanWrite
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

	public MessageWriter(SocketWrapper socketWrapper, bool appendSpliterString, RsaSocketDataSecurityFactory encryptHelper)
	{
		_socketWrapper = socketWrapper;
		_appendSpliterString = appendSpliterString;
		rsaSocketDataSecurityFactory = encryptHelper;
	}

	public bool Write<T>(MessageEx<T> message)
	{
		string text = JsonUtils.Stringify(message);
		bool flag = false;
		if (_socketWrapper != null && _socketWrapper.Connected)
		{
			string text2 = (_appendSpliterString ? MessageStringFormat(text) : text);
			text2 = rsaSocketDataSecurityFactory.EncryptToBase64(text2);
			byte[] bytes = Constants.Encoding.GetBytes(text2);
			int num = bytes.Length;
			byte[] bytes2 = BitConverter.GetBytes(num);
			int num2 = bytes2.Length;
			byte[] array = new byte[checked(num + num2)];
			Array.Copy(bytes2, 0, array, 0, num2);
			Array.Copy(bytes, 0, array, num2, num);
			flag = _socketWrapper.Write(array) > 0;
			if (message.Action == "addOrEditContactEx")
			{
				LogHelper.LogInstance.Debug("socket send: " + message.Action + " " + (flag ? "success" : "failed"));
			}
			else
			{
				LogHelper.LogInstance.Debug("socket send: " + text + " " + (flag ? "success" : "failed"));
			}
		}
		else if (message.Action == "addOrEditContactEx")
		{
			LogHelper.LogInstance.Debug("socket disconnected, send: " + message.Action + " failed");
		}
		else
		{
			LogHelper.LogInstance.Debug("socket disconnected, send: " + text + " failed");
		}
		return flag;
	}

	private string MessageStringFormat(string rawString)
	{
		return string.Format("{0}{1}", rawString, "\\r\\n");
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
