using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.socket;

public class AppServiceRequest
{
	private Socket mAppSocketClient;

	public RsaSocketDataSecurityFactory _RsaSocketEncryptHelper;

	private int mReceiveTimeout = 10000;

	internal Socket GetRawAppSocketClient => mAppSocketClient;

	private IPEndPoint RemoteEndPoint { get; set; }

	public AppServiceRequest(IPEndPoint remoteEndPoint, RsaSocketDataSecurityFactory encryptHelper)
	{
		RemoteEndPoint = remoteEndPoint;
		_RsaSocketEncryptHelper = encryptHelper;
	}

	public AppServiceResponse Request(int serviceCode, string methodName, Header header, Action<int, long, long> progress)
	{
		mAppSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		mAppSocketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, mReceiveTimeout);
		mAppSocketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, optionValue: true);
		mAppSocketClient.NoDelay = true;
		mAppSocketClient.ReceiveTimeout = mReceiveTimeout;
		mAppSocketClient.Connect(RemoteEndPoint);
		if (!mAppSocketClient.Connected)
		{
			return null;
		}
		Header header2 = header;
		if (header2 == null)
		{
			header2 = new Header();
		}
		header2.AddOrReplace("MethodName", methodName);
		string text = JsonConvert.SerializeObject(header2.Headers);
		string s = _RsaSocketEncryptHelper.EncryptToBase64(text);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] buffer = _RsaSocketEncryptHelper.EncryptBase64(BitConverter.GetBytes(bytes.Length));
		byte[] buffer2 = _RsaSocketEncryptHelper.EncryptBase64(BitConverter.GetBytes(serviceCode));
		mAppSocketClient.Send(buffer2);
		mAppSocketClient.Send(buffer);
		mAppSocketClient.Send(bytes);
		return new AppServiceResponse(this, _RsaSocketEncryptHelper);
	}

	private MemoryStream RequestStream(int serviceCode, string methodName, Header header)
	{
		AppServiceResponse appServiceResponse = null;
		try
		{
			if (header == null)
			{
				header = new Header();
			}
			appServiceResponse = Request(serviceCode, methodName, header, null);
			Header header2 = appServiceResponse.ReadHeader(null);
			if (!header2.ContainsKey("Status"))
			{
				throw new Exception("Response is null or response header dot not contains status key");
			}
			string @string = header2.GetString("Status");
			if (!"-6".Equals(@string))
			{
				throw new Exception("Response status is not normal, operate failed");
			}
			long @int = header2.GetInt64("StreamLength", 0L);
			header.AddOrReplace("Status", "-6");
			appServiceResponse.WriteHeader(header);
			MemoryStream buffer = new MemoryStream();
			appServiceResponse.ReadStreamOld(@int, delegate(byte[] bytes, int readLength, long readTotal, long fileLength)
			{
				buffer.Write(bytes, 0, readLength);
			});
			return buffer;
		}
		finally
		{
			if (appServiceResponse != null)
			{
				try
				{
					appServiceResponse.Dispose();
					appServiceResponse = null;
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public string RequestString(int serviceCode, string methodName, Header header)
	{
		MemoryStream memoryStream = RequestStream(serviceCode, methodName, header);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return Encoding.UTF8.GetString(memoryStream.ToArray());
	}
}
