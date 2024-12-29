using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

public class ODMServerMain
{
	public struct InputParameters
	{
		public string siteId;

		public string siteUrl;

		public string serialNumber;

		public string logId;

		public string prodId;

		public string keyType;

		public string keyName;

		public string clientReqType;

		public string userName;

		public string password;

		public string mascId;

		public string tokenconnectpath;
	}

	private static InputParameters inputParameters = default(InputParameters);

	private static Socket serverSocket;

	private static readonly List<Socket> clientSockets = new List<Socket>();

	private const int BUFFER_SIZE = 2048;

	private const int PORT = 5000;

	private static readonly byte[] buffer = new byte[2048];

	public static void InitParams(string username, string password, string imei)
	{
		inputParameters.userName = username;
		inputParameters.password = password;
		inputParameters.siteId = "RSA";
		inputParameters.siteUrl = "RSA";
		inputParameters.serialNumber = imei;
		inputParameters.logId = "91C44564934F4E96A1533641D2694B38";
		inputParameters.prodId = "M6102";
		inputParameters.keyType = "23";
		inputParameters.keyName = "1";
		inputParameters.clientReqType = "0x00";
		inputParameters.mascId = "GC-7777777-TEST-190";
	}

	public static void StartServer()
	{
		LogHelper.LogInstance.Debug("Start ODMSocketServer");
		try
		{
			SetupServer();
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Start ODMSocketServer error: " + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
		}
	}

	private static void SetupServer()
	{
		LogHelper.LogInstance.Debug("Setting up server...");
		serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));
		serverSocket.Listen(100);
		serverSocket.BeginAccept(AcceptCallback, null);
		LogHelper.LogInstance.Debug("Server started.....");
	}

	public static void CloseAllSockets()
	{
		if (serverSocket == null)
		{
			return;
		}
		foreach (Socket clientSocket in clientSockets)
		{
			try
			{
				clientSocket.Shutdown(SocketShutdown.Both);
				clientSocket.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("socket shutdown error:" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		serverSocket.Close();
		LogHelper.LogInstance.Debug("Close ODMSocketServer");
	}

	private static void AcceptCallback(IAsyncResult AR)
	{
		Socket socket;
		try
		{
			socket = serverSocket.EndAccept(AR);
		}
		catch (ObjectDisposedException)
		{
			return;
		}
		clientSockets.Add(socket);
		socket.BeginReceive(buffer, 0, 2048, SocketFlags.None, ReceiveCallback, socket);
		LogHelper.LogInstance.Debug("Client connected, waiting for request...");
		serverSocket.BeginAccept(AcceptCallback, null);
	}

	private static void ReceiveCallback(IAsyncResult AR)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		string sErrorMessage = string.Empty;
		Socket socket = (Socket)AR.AsyncState;
		int num;
		try
		{
			num = socket.EndReceive(AR);
		}
		catch (SocketException)
		{
			LogHelper.LogInstance.Debug("Client forcefully disconnected");
			socket.Close();
			clientSockets.Remove(socket);
			return;
		}
		byte[] array = new byte[num];
		Array.Copy(buffer, array, num);
		string @string = Encoding.ASCII.GetString(array);
		LogHelper.LogInstance.Debug(@string);
		string text = @string.Substring(0, 2);
		string text2 = CustomConvert.Instance.ByteArrayToString(array);
		if (text2 == "IsRunning")
		{
			byte[] array2 = CustomConvert.Instance.StringToByteArray("0");
			socket.Send(array2);
		}
		string text3 = text2.Substring(4);
		LogHelper.LogInstance.Debug("Get type: " + text);
		LogHelper.LogInstance.Debug("Received Phone Hash: " + text3);
		try
		{
			string text4 = signDataCollectPenang(inputParameters.serialNumber, inputParameters.logId, inputParameters.prodId, text, inputParameters.keyName, inputParameters.clientReqType, text3, inputParameters.userName, inputParameters.password, inputParameters.mascId, out sErrorMessage);
			LogHelper.LogInstance.Debug("GPS Signed Hash: " + text4);
			byte[] array3 = CustomConvert.Instance.StringToByteArray(text4);
			Array.Resize(ref array3, 512);
			socket.Send(array3);
			LogHelper.LogInstance.Debug("Signed HASH sent to SLA_Challenge");
		}
		catch
		{
			LogHelper.LogInstance.Debug("GPS Sign FAILED " + sErrorMessage);
		}
	}

	private static string signDataCollectPenang(string serialNumber, string logId, string prodId, string keyType, string keyName, string clientReqType, string hash, string username, string password, string mascid, out string sErrorMessage)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		sErrorMessage = string.Empty;
		LogHelper.LogInstance.Debug("Start to do DataSignODM:" + hash);
		string text = string.Empty;
		bool flag = true;
		for (int i = 0; i < 3; i++)
		{
			try
			{
				LogHelper.LogInstance.Debug("Before DataSignODM...");
				text = Web.DataSignODM(serialNumber, logId, clientReqType, prodId, keyType, keyName, hash, username, password, mascid);
			}
			catch (Exception ex)
			{
				sErrorMessage = ex.Message + Environment.NewLine + ex.StackTrace;
				LogHelper.LogInstance.Debug(sErrorMessage);
				Thread.Sleep(500);
				flag = false;
			}
			if (flag)
			{
				break;
			}
		}
		return text.ToLower();
	}
}
