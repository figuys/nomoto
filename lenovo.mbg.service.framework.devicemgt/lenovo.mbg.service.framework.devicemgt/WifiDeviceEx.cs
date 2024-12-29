using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

public class WifiDeviceEx : TcpAndroidDevice
{
	public WifiDeviceData DeviceData { get; set; }

	protected override ConnectErrorCode CheckAppVersion()
	{
		return ConnectErrorCode.OK;
	}

	public override ConnectErrorCode StartApp()
	{
		return ConnectErrorCode.OK;
	}

	protected override ConnectErrorCode InstallApp()
	{
		return ConnectErrorCode.OK;
	}

	protected override ConnectErrorCode UninstallApp()
	{
		return ConnectErrorCode.OK;
	}

	public override void CallAppToFrontstage()
	{
	}

	protected override WifiDeviceData GetServiceHostEndPointInfo()
	{
		return DeviceData;
	}

	public override ConnectErrorCode LoadProperty()
	{
		Load();
		if (Property != null)
		{
			return ConnectErrorCode.OK;
		}
		return ConnectErrorCode.PropertyLoadFail;
	}

	public override bool FocuseApp()
	{
		return true;
	}

	protected override bool CheckPermissionsPerpare()
	{
		return true;
	}

	public override void Load()
	{
		try
		{
			PropInfo propInfo = new PropInfoLoader().LoadAll(this);
			Property = new AndroidDeviceProperty(propInfo);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Load wifi device property failed:" + ex);
		}
	}
}
