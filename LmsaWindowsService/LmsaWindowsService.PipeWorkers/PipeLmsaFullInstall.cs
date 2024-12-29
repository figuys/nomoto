using System;
using System.Diagnostics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService.PipeWorkers;

public class PipeLmsaFullInstall : IPipeMessageWorker
{
	public void Do(object data)
	{
		try
		{
			GlobalFun.KillProcess("adb");
			GlobalFun.KillProcess("qcomdloader");
			string fileName = data.ToString();
			Process.Start(new ProcessStartInfo
			{
				FileName = fileName,
				UseShellExecute = false,
				CreateNoWindow = true,
				Verb = "runas"
			});
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("full install exception: ", exception);
		}
	}
}
