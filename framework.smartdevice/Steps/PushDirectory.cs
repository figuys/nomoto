using System;
using System.IO;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class PushDirectory : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		DeviceEx device = base.Recipe.Device;
		string deviceID = string.Empty;
		if (device != null)
		{
			deviceID = device.Identifer;
		}
		string text = base.Info.Args.LocalPath;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string path = base.Info.Args.DevicePath;
		string[] files = Directory.GetFiles(text, "*.zip", SearchOption.AllDirectories);
		foreach (string text2 in files)
		{
			string fileName = Path.GetFileName(text2);
			try
			{
				global::lenovo.mbg.service.framework.smartdevice.Smart.DeviceOperator.PushFile(deviceID, text2, Path.Combine(path, fileName));
			}
			catch (Exception)
			{
			}
		}
		base.Log.AddResult(this, Result.PASSED);
	}
}
