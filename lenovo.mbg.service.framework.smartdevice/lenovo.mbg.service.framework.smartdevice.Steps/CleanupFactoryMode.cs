using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class CleanupFactoryMode : BaseStep
{
	protected int timeout = 30000;

	protected StringBuilder sb = new StringBuilder();

	public override void Run()
	{
		int num = 0;
		if (base.Info.Args.RetryTimes != null)
		{
			num = base.Info.Args.RetryTimes;
		}
		if (base.Info.Args.Timeout != null)
		{
			timeout = base.Info.Args.Timeout;
		}
		Result result = Result.CLEAR_FACTORYMODE_FAILED;
		int num2 = 0;
		do
		{
			Clear();
			if (Check())
			{
				result = Result.PASSED;
				break;
			}
			Thread.Sleep(3000);
			base.Log.AddLog($"cleanup factory mode failed, try {++num2}", upload: true);
		}
		while (--num >= 0);
		string response = null;
		if (result != Result.PASSED)
		{
			response = "cleanup factory mode failed";
		}
		base.Log.AddResult(this, result, response);
	}

	protected void Clear()
	{
		string command = base.Info.Args.Command;
		command = EncapsulationFastbootCommand(command);
		string text = ProcessRunner.ProcessString(LoadToolPath(), command, timeout)?.ToLower();
		base.Log.AddLog("fastboot command:" + command + ",response: " + text, upload: true);
	}

	protected bool Check()
	{
		string text = EncapsulationFastbootCommand("oem config bootmode");
		string text2 = ProcessRunner.ProcessString(LoadToolPath(), text, timeout)?.ToLower();
		base.Log.AddLog("fastboot command: " + text + ", response: " + text2, upload: true);
		Regex regex = new Regex("<value>\\r\\n\\(bootloader\\)\\s*</value>\\r\\n");
		Regex regex2 = new Regex("<value>\\r\\n\\(bootloader\\)\\s*normal\\s*\\r\\n\\(bootloader\\)\\s*</value>\\r\\n");
		Regex regex3 = new Regex("<value>\\r\\n\\(bootloader\\)\\s*\\r\\n\\(bootloader\\)\\s*</value>\\r\\n");
		if (!regex.IsMatch(text2) && !regex2.IsMatch(text2))
		{
			return regex3.IsMatch(text2);
		}
		return true;
	}
}
