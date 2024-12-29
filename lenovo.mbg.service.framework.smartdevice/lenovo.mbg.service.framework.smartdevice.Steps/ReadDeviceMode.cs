using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ReadDeviceMode : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		DeviceEx device = base.Recipe.Device;
		string response = null;
		if (device != null)
		{
			response = (base.RunResult = device.ConnectType.ToString());
		}
		base.Log.AddResult(this, Result.PASSED, response);
	}
}
