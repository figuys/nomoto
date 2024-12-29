using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class FBConnectionMonitorEx
{
	private class FastbootScaner
	{
		private readonly FBConnectionMonitorEx outer;

		private readonly Dictionary<string, DeviceEx> Cache = new Dictionary<string, DeviceEx>();

		private volatile bool isRunning;

		public FastbootScaner(FBConnectionMonitorEx outer)
		{
			this.outer = outer;
		}

		public void Start()
		{
			isRunning = true;
			ScanDevice();
		}

		private void ScanDevice()
		{
			Cache.Clear();
			List<string> list = new List<string>();
			new List<string>();
			while (isRunning)
			{
				list = outer._operator.FindDevices();
				foreach (string item in list)
				{
					if (!Cache.ContainsKey(item))
					{
						FastbootDeviceEx fastbootDeviceEx = new FastbootDeviceEx
						{
							ConnectType = ConnectType.Fastboot,
							Identifer = item
						};
						Cache.Add(item, fastbootDeviceEx);
						outer.m_listener.OnConnect(fastbootDeviceEx, DevicePhysicalStateEx.Online);
					}
					else if (Cache[item].IsRemove)
					{
						Cache[item].IsRemove = false;
						outer.m_listener.OnConnect(Cache[item], DevicePhysicalStateEx.Online);
					}
				}
				foreach (string item2 in (from n in Cache
					where !n.Value.IsRemove
					select n.Key).Except(list).ToList())
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
					outer.m_listener.OnDisconnect(deviceEx);
				}
				Thread.Sleep(1000);
			}
		}

		public void Stop()
		{
			isRunning = false;
		}
	}

	private FastbootScaner scaner;

	private ICompositListener m_listener;

	private FastbootOperator _operator = new FastbootOperator();

	public FBConnectionMonitorEx(ICompositListener listener)
	{
		m_listener = listener;
		scaner = new FastbootScaner(this);
	}

	public void StartMonitoring()
	{
		scaner.Start();
	}

	public void StopMonitoring()
	{
		scaner.Stop();
	}
}
