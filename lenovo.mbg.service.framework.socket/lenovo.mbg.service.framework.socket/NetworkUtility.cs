using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.socket;

public static class NetworkUtility
{
	private class AvailablePort
	{
		public DateTime Time { get; set; }

		public int Prot { get; set; }
	}

	private class AvailablePortFactory
	{
		private List<AvailablePort> ports = new List<AvailablePort>();

		private static readonly object lockObj = new object();

		private Random R = new Random();

		private int minLockTime = 30000;

		public int GetAvailablePort(int begin, int end)
		{
			lock (lockObj)
			{
				int num = 0;
				while (IsPortInUse(num = R.Next(begin, end)) && !IsTimeLock(num))
				{
				}
				ports.Add(new AvailablePort
				{
					Prot = num,
					Time = DateTime.Now
				});
				return num;
			}
		}

		private bool IsTimeLock(int port)
		{
			lock (lockObj)
			{
				return ports.Exists((AvailablePort m) => m.Prot == port && (DateTime.Now - m.Time).Milliseconds < minLockTime);
			}
		}

		public bool IsPortInUse(int port)
		{
			bool result = false;
			IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
			for (int i = 0; i < activeTcpListeners.Length; i++)
			{
				if (activeTcpListeners[i].Port == port)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}

	private static List<string> ipArr = new List<string>();

	private static AvailablePortFactory availablePortFactory = new AvailablePortFactory();

	public static List<Tuple<string, string>> GetAllIPV4s()
	{
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if (networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
			{
				continue;
			}
			IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
			UnicastIPAddressInformationCollection unicastAddresses = iPProperties.UnicastAddresses;
			if (unicastAddresses.Count <= 0)
			{
				continue;
			}
			foreach (UnicastIPAddressInformation item2 in unicastAddresses)
			{
				if (item2.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					string text = item2.Address.ToString();
					if (!ipArr.Contains(text))
					{
						ipArr.Add(text);
						LogHelper.LogInstance.Info($"====>>Local adapter type: {networkInterface.NetworkInterfaceType}, ip:{text}");
					}
					string item = ((iPProperties.GatewayAddresses.Count == 0) ? string.Empty : iPProperties.GatewayAddresses[0].Address.ToString());
					list.Add(new Tuple<string, string>(text, item));
				}
			}
		}
		return list;
	}

	public static bool IsPortInUse(int port)
	{
		return availablePortFactory.IsPortInUse(port);
	}

	public static int GetAvailablePort(int begin, int end)
	{
		return availablePortFactory.GetAvailablePort(begin, end);
	}
}
