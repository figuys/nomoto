using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Device;

public abstract class AbstractDeviceConnectionManagerEx : IDisposable
{
	public abstract Action<dynamic> BeforeValidateEvent { get; set; }

	public abstract Action<dynamic> AfterValidateEvent { get; set; }

	public abstract DeviceEx MasterDevice { get; protected set; }

	public abstract List<Tuple<string, string>> WirelessWaitForConnectEndPoints { get; }

	public abstract IList<DeviceEx> ConntectedDevices { get; }

	public abstract event WirelessMornitoringAddressChangedHandler WifiMonitoringEndPointChanged;

	public abstract event EventHandler<DeviceEx> Connecte;

	public abstract event EventHandler<DeviceEx> DisConnecte;

	public abstract event EventHandler<MasterDeviceChangedEventArgs> MasterDeviceChanged;

	public abstract void Start();

	public abstract void Dispose();

	public abstract void SwitchDevice(string id);

	public abstract bool IsMasterConnectedByHelperForAndroid11();
}
