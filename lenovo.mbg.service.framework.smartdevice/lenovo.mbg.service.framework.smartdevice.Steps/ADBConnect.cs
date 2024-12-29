using System.Text.RegularExpressions;
using System.Threading;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ADBConnect : BaseStep
{
	public override void Run()
	{
		int num = 180;
		if (base.Info.Args.Timeout != null)
		{
			num = base.Info.Args.Timeout / 1000;
		}
		bool flag = false;
		while (!flag && num > 0)
		{
			if (GetDevice(ConnectType.Adb, delegate(object s)
			{
				DeviceEx deviceEx = s as DeviceEx;
				return !Regex.IsMatch(deviceEx.Identifer, "&|:") && deviceEx.PhysicalStatus == DevicePhysicalStateEx.Online;
			}) != null)
			{
				flag = true;
				break;
			}
			Thread.Sleep(5000);
			num -= 5;
		}
		Result result = Result.PASSED;
		string response = null;
		if (!flag)
		{
			result = Result.ADB_CONNECT_FAILED;
			response = "connect timeout";
		}
		base.Log.AddResult(this, result, response);
	}
}
