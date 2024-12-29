using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class RunODMSocketServer : BaseStep
{
	public override void Run()
	{
		DeviceEx device = base.Recipe.Device;
		string username = null;
		string password = null;
		string imei = null;
		if (device != null)
		{
			imei = device.Property.IMEI1;
		}
		if (base.Info.Args.PARAMS != null)
		{
			dynamic val = base.Info.Args.PARAMS;
			username = val.username;
			password = val.password;
		}
		ODMServerMain.InitParams(username, password, imei);
		ODMServerMain.StartServer();
		base.Log.AddResult(this, Result.PASSED);
	}
}
