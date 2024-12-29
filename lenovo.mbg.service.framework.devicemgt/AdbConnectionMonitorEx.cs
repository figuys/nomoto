using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;
using SharpAdbClient;

namespace lenovo.mbg.service.framework.devicemgt;

internal class AdbConnectionMonitorEx : IDisposable
{
	private class AdbScaner
	{
		private readonly AdbConnectionMonitorEx outer;

		private readonly Dictionary<string, DeviceEx> Cache = new Dictionary<string, DeviceEx>();

		private volatile bool isRunning;

		protected AdbOperator Operator = new AdbOperator();

		private static string wmiDeviceQuery = "SELECT * FROM Win32_PnPEntity WHERE ClassGuid='{eec5ad98-8080-425f-922a-dabf3de3f69a}'";

		private ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher(wmiDeviceQuery);

		private string[] manufacturers = new string[2] { "LENOVO", "MOTOROLA" };

		public AdbScaner(AdbConnectionMonitorEx outer)
		{
			this.outer = outer;
		}

		public void Start()
		{
			isRunning = true;
			Cache.Clear();
			List<DeviceDataEx> foundDevices = new List<DeviceDataEx>();
			new List<DeviceData>();
			new List<string>();
			while (isRunning)
			{
				foundDevices.Clear();
				List<DeviceData> list = Operator.FindAdbDevices();
				DeviceDataEx deviceDataEx = findNewDeviceByWmi();
				if (deviceDataEx != null)
				{
					foundDevices.Add(deviceDataEx);
				}
				list.ForEach(delegate(DeviceData n)
				{
					foundDevices.Add(new DeviceDataEx
					{
						Data = n,
						PhyState = (DevicePhysicalStateEx)n.State
					});
				});
				foreach (DeviceDataEx item in foundDevices)
				{
					if (!Cache.ContainsKey(item.Data.Serial))
					{
						AdbDeviceEx adbDeviceEx = new AdbDeviceEx
						{
							ConnectType = ConnectType.Adb,
							Identifer = item.Data.Serial,
							BeforeValidateAction = (outer.m_Listener as DeviceConnectionManagerEx).BeforeValidateEvent,
							AfterValidateAction = (outer.m_Listener as DeviceConnectionManagerEx).AfterValidateEvent
						};
						Cache.Add(item.Data.Serial, adbDeviceEx);
						outer.m_Listener.OnConnect(adbDeviceEx, item.PhyState);
					}
					else if (Cache[item.Data.Serial].IsRemove)
					{
						Cache[item.Data.Serial].IsRemove = false;
						outer.m_Listener.OnConnect(Cache[item.Data.Serial], item.PhyState);
					}
					else
					{
						Cache[item.Data.Serial].PhysicalStatus = item.PhyState;
					}
				}
				foreach (string item2 in (from n in Cache
					where !n.Value.IsRemove
					select n.Key).Except(foundDevices.Select((DeviceDataEx n) => n.Data.Serial)).ToList())
				{
					DeviceEx deviceEx = Cache[item2];
					if (deviceEx.WorkType == DeviceWorkType.None)
					{
						Cache.Remove(item2);
					}
					else
					{
						deviceEx.IsRemove = true;
					}
					outer.m_Listener.OnDisconnect(deviceEx);
				}
				Thread.Sleep(1000);
			}
		}

		public void Stop()
		{
			isRunning = false;
		}

		private DeviceDataEx findNewDeviceByWmi()
		{
			try
			{
				foreach (ManagementObject item in wmiSearcher.Get())
				{
					object obj2 = item["Manufacturer"];
					object obj3 = item["PNPDeviceID"];
					string empty = string.Empty;
					if (obj2 != null && obj3 != null && manufacturers.Contains(obj2.ToString().ToUpper()) && (empty = obj3.ToString()).StartsWith("USB"))
					{
						return new DeviceDataEx
						{
							Data = new DeviceData
							{
								Serial = empty
							},
							PhyState = DevicePhysicalStateEx.UsbDebugSwitchClosed
						};
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	}

	public static IAdbClient m_AdbClient = new AdbClient();

	private readonly ICompositListener m_Listener;

	private readonly string m_AdbExeFileFullName;

	private readonly AdbScaner adbScaner;

	public AdbConnectionMonitorEx(ICompositListener listener, string adbFileName)
	{
		m_Listener = listener;
		m_AdbExeFileFullName = adbFileName;
		adbScaner = new AdbScaner(this);
	}

	public void StartMonitoring()
	{
		AdbServer.Instance.StartServer(m_AdbExeFileFullName, restartServerIfNewer: true);
		adbScaner.Start();
	}

	public void StopMonitoring()
	{
		adbScaner.Stop();
		TryKillAdb();
	}

	private void TryKillAdb()
	{
		try
		{
			if (m_AdbClient != null)
			{
				m_AdbClient.KillAdb();
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Try to kill adb throw exception " + ex.ToString());
		}
	}

	public void Dispose()
	{
		StopMonitoring();
	}
}
