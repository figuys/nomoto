using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class AndroidShellVerify : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		DeviceEx device = base.Recipe.Device;
		string deviceID = null;
		if (device != null)
		{
			deviceID = device.Identifer;
		}
		string command = base.Info.Args.Command;
		string text = base.Info.Args.Ref;
		string text2 = global::Smart.DeviceOperator.Shell(deviceID, command).Trim();
		bool flag = false;
		flag = ((!(text == string.Empty)) ? (text2 == text) : (text2 != string.Empty));
		Result result = (flag ? Result.PASSED : Result.FAILED);
		base.Log.AddResult(this, result);
	}
}
