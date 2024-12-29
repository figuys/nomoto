using System;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.services.Device;

public abstract class DeviceEx
{
	private DeviceWorkType workType = DeviceWorkType.None;

	private volatile DevicePhysicalStateEx physicalStatus = DevicePhysicalStateEx.None;

	private volatile DeviceSoftStateEx lockedSoftStatus = DeviceSoftStateEx.None;

	private volatile DeviceSoftStateEx releaseSoftStatusCondition = DeviceSoftStateEx.None;

	private volatile bool softStatusIsLocked;

	private readonly object softStatusLockOjb = new object();

	private volatile bool autoRelease;

	private volatile DeviceSoftStateEx softStatus = DeviceSoftStateEx.None;

	public string Identifer { get; set; }

	public ConnectType ConnectType { get; set; }

	public string ConnectTime { get; set; }

	public IDeviceOperator DeviceOperator { get; set; }

	public DeviceType DeviceType { get; set; }

	public abstract IAndroidDevice Property { get; set; }

	public string ConnectedAppType { get; set; }

	public int AppVersion { get; set; }

	public Action<dynamic> BeforeValidateAction { get; set; }

	public Action<dynamic> AfterValidateAction { get; set; }

	public DeviceWorkType WorkType
	{
		get
		{
			return workType;
		}
		set
		{
			if (workType != value)
			{
				DeviceWorkType deviceWorkType = workType;
				workType = value;
				LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, work type: {deviceWorkType} --> {workType}======");
			}
		}
	}

	public bool IsRemove { get; set; }

	public DevicePhysicalStateEx PhysicalStatus
	{
		get
		{
			return physicalStatus;
		}
		set
		{
			if (physicalStatus != value)
			{
				DevicePhysicalStateEx devicePhysicalStateEx = physicalStatus;
				physicalStatus = value;
				LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, phy status: {devicePhysicalStateEx} --> {physicalStatus}======");
				OnPhysicalStatusChanged(devicePhysicalStateEx, physicalStatus);
				FirePhysicalStatusChangedEvent(this, physicalStatus);
				if (physicalStatus == DevicePhysicalStateEx.Offline)
				{
					SoftStatus = DeviceSoftStateEx.Offline;
				}
			}
		}
	}

	public bool SoftStatusIsLocked
	{
		get
		{
			return softStatusIsLocked;
		}
		private set
		{
			softStatusIsLocked = value;
		}
	}

	public DeviceSoftStateEx SoftStatus
	{
		get
		{
			return softStatus;
		}
		set
		{
			if (softStatus != value)
			{
				DeviceSoftStateEx deviceSoftStateEx = softStatus;
				softStatus = value;
				LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, soft status: {deviceSoftStateEx} --> {softStatus}======");
				OnSoftStatusChanged(deviceSoftStateEx, softStatus);
				FireSoftStatusChangedEvent(this, softStatus);
			}
		}
	}

	public Func<bool> InstallAppCallback { get; set; }

	public Action RetryConnectCallback { get; set; }

	private event EventHandler<DevicePhysicalStateEx> physicalStatusChanged;

	public event EventHandler<DevicePhysicalStateEx> PhysicalStatusChanged
	{
		add
		{
			physicalStatusChanged += value;
			value.BeginInvoke(this, PhysicalStatus, null, null);
		}
		remove
		{
			physicalStatusChanged -= value;
		}
	}

	private event EventHandler<DeviceSoftStateEx> softStatusChanged;

	public event EventHandler<DeviceSoftStateEx> SoftStatusChanged
	{
		add
		{
			softStatusChanged += value;
			value.BeginInvoke(this, SoftStatus, null, null);
		}
		remove
		{
			softStatusChanged -= value;
		}
	}

	public DeviceEx()
	{
		DeviceType = DeviceType.Normal;
	}

	private void FirePhysicalStatusChangedEvent(object sender, DevicePhysicalStateEx e)
	{
		if (this.physicalStatusChanged != null)
		{
			Delegate[] invocationList = this.physicalStatusChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<DevicePhysicalStateEx>)invocationList[i]).BeginInvoke(sender, e, null, null);
			}
		}
	}

	public abstract void Load();

	protected virtual void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
	{
	}

	public void LockSoftStatus(bool autoRelease, DeviceSoftStateEx when)
	{
		lock (softStatusLockOjb)
		{
			releaseSoftStatusCondition = when;
			this.autoRelease = autoRelease;
			softStatusIsLocked = true;
		}
	}

	public void ReleaseSoftStatusLock()
	{
		lock (softStatusLockOjb)
		{
			softStatusIsLocked = false;
			if (lockedSoftStatus != DeviceSoftStateEx.None)
			{
				SoftStatus = lockedSoftStatus;
			}
		}
	}

	private void FireSoftStatusChangedEvent(object sender, DeviceSoftStateEx e)
	{
		EventHandler<DeviceSoftStateEx> eventHandler = this.softStatusChanged;
		if (eventHandler != null)
		{
			Delegate[] invocationList = eventHandler.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<DeviceSoftStateEx>)invocationList[i]).BeginInvoke(sender, e, null, null);
			}
		}
	}

	protected virtual void OnSoftStatusChanged(DeviceSoftStateEx prev, DeviceSoftStateEx current)
	{
	}

	public bool IsHWEnable()
	{
		if (softStatus == DeviceSoftStateEx.Online)
		{
			if (!(ConnectedAppType == "Ma"))
			{
				if (ConnectedAppType == "Moto")
				{
					return AppVersion >= 1200000;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return $"device: {Identifer}#{ConnectTime}, modelname:{Property?.ModelName}, connect type: {ConnectType}, phy status: {PhysicalStatus}, soft status: {SoftStatus}, app type: {ConnectedAppType}";
	}

	public void UnloadEvent()
	{
		this.softStatusChanged = null;
		this.physicalStatusChanged = null;
		BeforeValidateAction = null;
		AfterValidateAction = null;
	}
}
