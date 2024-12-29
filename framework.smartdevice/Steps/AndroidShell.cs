using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class AndroidShell : BaseStep
{
	public override void Run()
	{
		DeviceEx device = base.Recipe.Device;
		string text = base.Info.Args.Command;
		string empty = string.Empty;
		if (text == "reboot bootloader")
		{
			empty = global::lenovo.mbg.service.framework.smartdevice.Smart.DeviceOperator.Command(text, -1, device?.Identifer);
			base.Log.AddLog("adb command: reboot bootloader, response: " + empty, upload: true);
		}
		else
		{
			empty = global::lenovo.mbg.service.framework.smartdevice.Smart.DeviceOperator.Shell(device?.Identifer, text);
			base.Log.AddLog("adb command: " + text + ", response: " + empty, upload: true);
		}
		if (base.Info.Args.PromptText != null)
		{
			string message = base.Info.Args.PromptText.ToString();
			string name = base.Info.Name;
			base.Recipe.UcDevice.MessageBox.Show(name, message).Wait();
		}
		base.RunResult = empty;
		if (text == "reboot edl")
		{
			base.Cache.Add("auto9008", true);
			if (empty.ToLower().Contains("error") || empty.ToLower().Contains("failed"))
			{
				base.RunResult = Result.FAILED.ToString();
			}
			else
			{
				base.RunResult = Result.PASSED.ToString();
			}
		}
		base.Log.AddResult(this, Result.PASSED);
	}
}
