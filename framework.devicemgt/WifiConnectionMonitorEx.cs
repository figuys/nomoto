using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.devicemgt;

internal class WifiConnectionMonitorEx : IDisposable
{
	private class AppSocketServiceConfig
	{
		[JsonProperty("ip")]
		public string Ip { get; set; }

		[JsonProperty("cmdPort")]
		public int CmdPort { get; set; }

		[JsonProperty("dataProt")]
		public int DataProt { get; set; }

		[JsonProperty("extendDataPort")]
		public int ExtendDataPort { get; set; }

		[JsonProperty("isForcedRestart")]
		public bool IsForcedRestart { get; set; }

		[JsonProperty("appType")]
		public string ConnectedAppType { get; set; }

		public int appVersion { get; set; }
	}

	private class NetworkAdapterWatcher
	{
		public delegate void NetworkAdapterChangeHandler(List<Tuple<string, string>> add, List<Tuple<string, string>> remove);

		private List<string> m_Unusables = new List<string>();

		private List<Tuple<string, string>> m_IPs = new List<Tuple<string, string>>();

		private Timer m_Timer;

		private NetworkAdapterChangeHandler m_Handler;

		public List<Tuple<string, string>> IPs
		{
			get
			{
				return new List<Tuple<string, string>>(m_IPs);
			}
			private set
			{
				m_IPs = value;
			}
		}

		public NetworkAdapterWatcher(NetworkAdapterChangeHandler handler)
		{
			m_Handler = handler;
		}

		public void Start()
		{
			m_Timer = new Timer(RefreshProc, null, 0, 5000);
		}

		public void Stop()
		{
			m_Timer.Dispose();
			m_Timer = null;
		}

		private void RefreshProc(object obj)
		{
			lock (this)
			{
				List<Tuple<string, string>> allIPV4s = NetworkUtility.GetAllIPV4s();
				List<Tuple<string, string>> list = allIPV4s.Except(IPs).ToList();
				List<Tuple<string, string>> list2 = IPs.Except(allIPV4s).ToList();
				if (list.Count > 0 || list2.Count > 0)
				{
					IPs = allIPV4s;
					m_Handler?.Invoke(list, list2);
				}
			}
		}
	}

	private class WaitForConnectWrapper : IDisposable
	{
		private SortedList<string, ListeningSocket> m_WaitForConnects;

		private readonly object m_WaitForConnectsLock = new object();

		public List<Tuple<string, string>> GetIpGateWayArr
		{
			get
			{
				lock (m_WaitForConnectsLock)
				{
					return m_WaitForConnects.Select((KeyValuePair<string, ListeningSocket> p) => new Tuple<string, string>(p.Key, p.Value.GateWay)).ToList();
				}
			}
		}

		public WaitForConnectWrapper()
		{
			m_WaitForConnects = new SortedList<string, ListeningSocket>();
		}

		public void Dispose()
		{
			lock (m_WaitForConnectsLock)
			{
				if (m_WaitForConnects == null)
				{
					return;
				}
				foreach (ListeningSocket value in m_WaitForConnects.Values)
				{
					value.Close();
				}
				m_WaitForConnects.Clear();
			}
		}

		public ListeningSocket GetValue(string key)
		{
			lock (m_WaitForConnectsLock)
			{
				if (m_WaitForConnects.ContainsKey(key))
				{
					return m_WaitForConnects[key];
				}
				return null;
			}
		}

		public void AddRange(SortedList<string, ListeningSocket> items)
		{
			lock (m_WaitForConnectsLock)
			{
				foreach (KeyValuePair<string, ListeningSocket> item in items)
				{
					m_WaitForConnects.Add(item.Key, item.Value);
				}
			}
		}

		public void Remove(string key)
		{
			lock (m_WaitForConnectsLock)
			{
				if (m_WaitForConnects.ContainsKey(key))
				{
					m_WaitForConnects.Remove(key);
				}
			}
		}

		public bool TryRemoveAndClose(string ip)
		{
			lock (m_WaitForConnectsLock)
			{
				KeyValuePair<string, ListeningSocket> keyValuePair = default(KeyValuePair<string, ListeningSocket>);
				KeyValuePair<string, ListeningSocket> keyValuePair2 = m_WaitForConnects.FirstOrDefault((KeyValuePair<string, ListeningSocket> p) => p.Key.Contains(ip));
				if (keyValuePair2.Equals(keyValuePair))
				{
					return false;
				}
				m_WaitForConnects.Remove(keyValuePair2.Key);
				keyValuePair2.Value?.Close();
				return true;
			}
		}
	}

	private class ListeningSocket
	{
		public IPAddress Address { get; set; }

		public int Port { get; set; }

		public Socket Listener { get; set; }

		public string GateWay { get; set; }

		public override string ToString()
		{
			return Address?.ToString() + ":" + Port;
		}

		public void Close()
		{
			if (Listener == null)
			{
				return;
			}
			try
			{
				LogHelper.LogInstance.Info($"Wifi socket listener {Address}:{Port} closed!");
				Listener.Close();
				Listener.Dispose();
				Listener = null;
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Error($"Close socket listener {Address}:{Port} failed, throw exception: {arg}");
			}
		}
	}

	private ICompositListener m_Listener;

	private NetworkAdapterWatcher m_NetworkWatcher;

	private WaitForConnectWrapper m_waitForConnect;

	private volatile bool _connecting;

	public string TAG => GetType().ToString();

	public List<Tuple<string, string>> WaitForConnectEndPoints => m_waitForConnect.GetIpGateWayArr;

	public WifiConnectionMonitorEx(ICompositListener listener)
	{
		m_Listener = listener;
		m_waitForConnect = new WaitForConnectWrapper();
	}

	public void StartMonitoring()
	{
		m_NetworkWatcher = new NetworkAdapterWatcher(NetworkAdapterChangeHandler);
		m_NetworkWatcher.Start();
	}

	public void StopMonitoring()
	{
		if (m_NetworkWatcher != null)
		{
			m_NetworkWatcher.Stop();
			m_NetworkWatcher = null;
		}
		if (m_waitForConnect != null)
		{
			m_waitForConnect.Dispose();
		}
	}

	public void Dispose()
	{
		StopMonitoring();
	}

	public void NetworkAdapterChangeHandler(List<Tuple<string, string>> add, List<Tuple<string, string>> remove)
	{
		bool changed = false;
		SortedList<string, ListeningSocket> sortedList = new SortedList<string, ListeningSocket>();
		foreach (Tuple<string, string> item in add)
		{
			int availablePort = NetworkUtility.GetAvailablePort(10000, 20000);
			ListeningSocket listeningSocket = LaunchWaitForConnectTask(item.Item1, availablePort);
			if (listeningSocket != null)
			{
				changed = true;
				listeningSocket.GateWay = item.Item2;
				string text = $"{item.Item1}:{availablePort}";
				sortedList.Add(text, listeningSocket);
				LogHelper.LogInstance.Debug($"ADD new wifi socket listener [{text}]");
			}
		}
		m_waitForConnect.AddRange(sortedList);
		remove.ForEach(delegate(Tuple<string, string> p)
		{
			changed |= m_waitForConnect.TryRemoveAndClose(p.Item1);
		});
		if (changed)
		{
			m_Listener.OnWifiMonitoringEndPointChanged(m_waitForConnect.GetIpGateWayArr);
		}
	}

	private ListeningSocket LaunchWaitForConnectTask(string ip, int port)
	{
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		ListeningSocket listeningSocket = new ListeningSocket
		{
			Address = IPAddress.Parse(ip),
			Port = port,
			Listener = socket
		};
		try
		{
			socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
			socket.Listen(1);
			socket.BeginAccept(CmdClientConnectionRequestHandler, listeningSocket);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error($"Add a new listener {ip}:{port} failed! Error: {ex.Message}");
			listeningSocket.Close();
			listeningSocket = null;
		}
		return listeningSocket;
	}

	private void CmdClientConnectionRequestHandler(IAsyncResult ar)
	{
		ListeningSocket listeningSocket = ar.AsyncState as ListeningSocket;
		if (_connecting)
		{
			return;
		}
		_connecting = true;
		Socket socket = null;
		try
		{
			if (listeningSocket.Listener == null)
			{
				return;
			}
			socket = listeningSocket.Listener.EndAccept(ar);
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, optionValue: true);
			socket.ReceiveTimeout = 40000;
			byte[] array = new byte[1024];
			if (!socket.Connected || socket.Receive(array, 0, 4, SocketFlags.None) != 4)
			{
				return;
			}
			int num = BitConverter.ToInt32(array, 0);
			if (num != socket.Receive(array, 0, num, SocketFlags.None))
			{
				return;
			}
			string @string = Encoding.UTF8.GetString(array, 0, num);
			LogHelper.LogInstance.Info("wifi device raw data: " + @string);
			AppSocketServiceConfig appSocketServiceConfig = JsonConvert.DeserializeObject<AppSocketServiceConfig>(@string);
			WifiDeviceData wifiDeviceData = new WifiDeviceData();
			wifiDeviceData.Ip = appSocketServiceConfig.Ip;
			wifiDeviceData.CmdPort = appSocketServiceConfig.CmdPort;
			wifiDeviceData.DataPort = appSocketServiceConfig.DataProt;
			wifiDeviceData.ExtendDataPort = appSocketServiceConfig.ExtendDataPort;
			wifiDeviceData.IpRSA = (socket.LocalEndPoint as IPEndPoint).Address.ToString();
			wifiDeviceData.ConnectedAppType = (string.IsNullOrEmpty(appSocketServiceConfig.ConnectedAppType) ? "Moto" : "Ma");
			WifiDeviceEx device = new WifiDeviceEx();
			device.DeviceData = wifiDeviceData;
			device.Identifer = $"{wifiDeviceData.Ip}:{wifiDeviceData.CmdPort}";
			device.BeforeValidateAction = (m_Listener as DeviceConnectionManagerEx).BeforeValidateEvent;
			device.AfterValidateAction = (m_Listener as DeviceConnectionManagerEx).AfterValidateEvent;
			device.ConnectType = ConnectType.Wifi;
			device.AppVersion = appSocketServiceConfig.appVersion;
			device.ConnectedAppType = wifiDeviceData.ConnectedAppType;
			device.SoftStatusChanged += delegate(object s, DeviceSoftStateEx e)
			{
				if (e == DeviceSoftStateEx.Offline || e == DeviceSoftStateEx.ManualDisconnect)
				{
					m_Listener.OnDisconnect(device);
				}
			};
			m_Listener.OnConnect(device, DevicePhysicalStateEx.Online);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Info("Software Fix try to read config from new device failed! Error: " + ex.Message);
		}
		finally
		{
			_connecting = false;
			socket?.Close();
			try
			{
				listeningSocket?.Listener?.BeginAccept(CmdClientConnectionRequestHandler, listeningSocket);
			}
			catch
			{
				listeningSocket.Close();
				listeningSocket = null;
			}
		}
	}
}
