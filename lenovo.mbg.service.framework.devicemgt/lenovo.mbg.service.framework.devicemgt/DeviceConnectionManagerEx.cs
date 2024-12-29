using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class DeviceConnectionManagerEx : AbstractDeviceConnectionManagerEx, ICompositListener, IPhysicalConnectionListener, INetworkAdapterListener
{
	private AdbConnectionMonitorEx adbListerner;

	private WifiConnectionMonitorEx wifiListener;

	private FBConnectionMonitorEx fastbootListener;

	private readonly object sync = new object();

	private readonly IList<DeviceEx> conntectedDevices = new List<DeviceEx>();

	private DeviceEx masterDevice;

	private EventHandler<MasterDeviceChangedEventArgs> masterDeviceChanged;

	private MasterDeviceChangedEventArgs masterDeviceChangedEventArgs = new MasterDeviceChangedEventArgs(null, null);

	public static bool IsLogOut { get; set; }

	public override List<Tuple<string, string>> WirelessWaitForConnectEndPoints
	{
		get
		{
			if (wifiListener == null)
			{
				return null;
			}
			return wifiListener.WaitForConnectEndPoints;
		}
	}

	public override IList<DeviceEx> ConntectedDevices
	{
		get
		{
			lock (sync)
			{
				return conntectedDevices.ToList();
			}
		}
	}

	public override DeviceEx MasterDevice
	{
		get
		{
			return masterDevice;
		}
		protected set
		{
			if (masterDevice != value)
			{
				DeviceEx deviceEx = masterDevice;
				if (deviceEx != null)
				{
					deviceEx.DeviceType = DeviceType.Slave;
				}
				masterDevice = value;
				if (masterDevice != null)
				{
					masterDevice.DeviceType = DeviceType.Master;
				}
				if (deviceEx != null && masterDevice != null)
				{
					LogHelper.LogInstance.Info("======chang master device:  " + deviceEx.Identifer + "#" + deviceEx.ConnectTime + ", " + deviceEx.Property?.ModelName + " --> " + masterDevice.Identifer + "#" + masterDevice.ConnectTime + ", " + masterDevice.Property?.ModelName + "======");
				}
				else if (deviceEx != null)
				{
					LogHelper.LogInstance.Info("======chang master device:  " + deviceEx.Identifer + "#" + deviceEx.ConnectTime + ", " + deviceEx.Property?.ModelName + " --> NULL======");
				}
				else if (masterDevice != null)
				{
					LogHelper.LogInstance.Info("======chang master device:  NULL --> " + masterDevice.Identifer + "#" + masterDevice.ConnectTime + ", " + masterDevice.Property?.ModelName + "======");
				}
				else
				{
					LogHelper.LogInstance.Info("======chang master device: NULL --> NULL======");
				}
				FireMasterDeviceChangedEvent(this, new MasterDeviceChangedEventArgs(deviceEx, masterDevice));
			}
		}
	}

	public override Action<dynamic> BeforeValidateEvent { get; set; }

	public override Action<dynamic> AfterValidateEvent { get; set; }

	public override event WirelessMornitoringAddressChangedHandler WifiMonitoringEndPointChanged;

	private event EventHandler<DeviceEx> connecte;

	public override event EventHandler<DeviceEx> Connecte
	{
		add
		{
			connecte += value;
			foreach (DeviceEx conntectedDevice in ConntectedDevices)
			{
				Delegate[] invocationList = this.connecte.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((EventHandler<DeviceEx>)invocationList[i]).BeginInvoke(this, conntectedDevice, null, null);
				}
			}
		}
		remove
		{
			connecte -= value;
		}
	}

	private event EventHandler<DeviceEx> disconnecte;

	public override event EventHandler<DeviceEx> DisConnecte
	{
		add
		{
			disconnecte += value;
		}
		remove
		{
			disconnecte -= value;
		}
	}

	public override event EventHandler<MasterDeviceChangedEventArgs> MasterDeviceChanged
	{
		add
		{
			masterDeviceChanged = (EventHandler<MasterDeviceChangedEventArgs>)Delegate.Combine(masterDeviceChanged, value);
			value.BeginInvoke(this, masterDeviceChangedEventArgs, null, null);
		}
		remove
		{
			masterDeviceChanged = (EventHandler<MasterDeviceChangedEventArgs>)Delegate.Remove(masterDeviceChanged, value);
		}
	}

	public DeviceConnectionManagerEx()
	{
		Port5037Check();
		adbListerner = new AdbConnectionMonitorEx(this, "adb.exe");
		wifiListener = new WifiConnectionMonitorEx(this);
		fastbootListener = new FBConnectionMonitorEx(this);
	}

	public void OnConnect(DeviceEx device, DevicePhysicalStateEx phyState)
	{
		lock (sync)
		{
			device.ConnectTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
			LogHelper.LogInstance.Info($"======device connected: {device.Identifer}#{device.ConnectTime}, connect type: {device.ConnectType}, app type: {device.ConnectedAppType}, work type: {device.WorkType}======");
			device.PhysicalStatus = phyState;
			conntectedDevices.Add(device);
			if (MasterDevice == null && device.ConnectType != ConnectType.Fastboot)
			{
				MasterDevice = device;
			}
			if (this.connecte != null)
			{
				Delegate[] invocationList = this.connecte.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((EventHandler<DeviceEx>)invocationList[i]).BeginInvoke(this, device, null, null);
				}
			}
		}
	}

	public void OnDisconnect(DeviceEx device)
	{
		lock (sync)
		{
			LogHelper.LogInstance.Info($"======device removed: {device.Identifer}#{device.ConnectTime}, modelname: {device.Property?.ModelName}, connect type: {device.ConnectType}, app type: {device.ConnectedAppType}, work type: {device.WorkType}, remove time: {DateTime.Now:yyyy-MM-ddTHH:mm:ss.ffffff}======");
			int num = conntectedDevices.IndexOf(device);
			if (num >= 0)
			{
				conntectedDevices.RemoveAt(num);
			}
			device.PhysicalStatus = DevicePhysicalStateEx.Offline;
			if (MasterDevice == null || MasterDevice.Identifer == device.Identifer)
			{
				MasterDevice = conntectedDevices.FirstOrDefault((DeviceEx m) => m.ConnectType != ConnectType.Fastboot && m.PhysicalStatus != DevicePhysicalStateEx.Offline);
			}
			if (this.disconnecte != null)
			{
				Delegate[] invocationList = this.disconnecte.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((EventHandler<DeviceEx>)invocationList[i]).BeginInvoke(this, device, null, null);
				}
			}
		}
	}

	public void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> endpoints)
	{
		WifiMonitoringEndPointChanged?.Invoke(endpoints);
	}

	public override void Start()
	{
		IsLogOut = false;
		conntectedDevices.Clear();
		Task.Run(delegate
		{
			adbListerner.StartMonitoring();
		});
		Task.Run(delegate
		{
			wifiListener.StartMonitoring();
		});
		Task.Run(delegate
		{
			fastbootListener.StartMonitoring();
		});
	}

	private bool Port5037Check()
	{
		foreach (string item in ProcessRunner.ProcessList("netstat.exe", "-nao", 3000))
		{
			if (!Regex.IsMatch(item, ".+?:5037\\s+.+"))
			{
				continue;
			}
			string[] array = item.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			int num = int.Parse(array[array.Length - 1]);
			if (num > 0)
			{
				try
				{
					Process.GetProcessById(num).Kill();
					return true;
				}
				catch (Exception exception)
				{
					LogHelper.LogInstance.Error("kill port 5037 failed", exception);
				}
			}
		}
		return false;
	}

	public override void Dispose()
	{
		IsLogOut = true;
		adbListerner.StopMonitoring();
		wifiListener.StopMonitoring();
		fastbootListener.StopMonitoring();
		this.connecte = null;
		this.disconnecte = null;
		masterDeviceChanged = null;
		BeforeValidateEvent = null;
		AfterValidateEvent = null;
		if (conntectedDevices.Count > 0)
		{
			foreach (DeviceEx conntectedDevice in conntectedDevices)
			{
				(conntectedDevice as TcpAndroidDevice)?.MessageManager?.Dispose();
				conntectedDevice.UnloadEvent();
			}
			conntectedDevices.Clear();
		}
		MasterDevice = null;
	}

	public override void SwitchDevice(string id)
	{
		DeviceEx deviceEx = ConntectedDevices.FirstOrDefault((DeviceEx m) => m.Identifer == id);
		MasterDevice = deviceEx;
	}

	private void FireMasterDeviceChangedEvent(object sender, MasterDeviceChangedEventArgs e)
	{
		masterDeviceChangedEventArgs = e;
		EventHandler<MasterDeviceChangedEventArgs> eventHandler = masterDeviceChanged;
		if (eventHandler != null)
		{
			Delegate[] invocationList = eventHandler.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<MasterDeviceChangedEventArgs>)invocationList[i]).BeginInvoke(this, e, null, null);
			}
		}
	}

	public override bool IsMasterConnectedByHelperForAndroid11()
	{
		if (masterDevice == null || !(masterDevice is TcpAndroidDevice))
		{
			return false;
		}
		TcpAndroidDevice tcpAndroidDevice = masterDevice as TcpAndroidDevice;
		if (tcpAndroidDevice.ConnectedAppType == "Moto" && tcpAndroidDevice.Property.AndroidVersion.Trim().StartsWith("11"))
		{
			return true;
		}
		return false;
	}
}
