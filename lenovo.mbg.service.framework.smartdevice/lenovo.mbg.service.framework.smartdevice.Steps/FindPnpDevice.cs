using System.Collections.Generic;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FindPnpDevice : BaseStep
{
	public override void Run()
	{
		bool running = true;
		bool quit = false;
		Task.Run(delegate
		{
			dynamic val = base.Info.Args.Image?.ToString();
			dynamic val2 = base.Info.Args.PromptText.ToString();
			if (base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, val2, val, showClose: true, popupWhenClose: true) == null)
			{
				quit = true;
			}
		});
		Task<bool> task = Task.Run(delegate
		{
			do
			{
				string text = base.Info.Args.DeviceName;
				List<string> comInfo = GlobalFun.GetComInfo("SELECT * FROM Win32_PnPEntity where Name like '%" + text + "%'");
				if (comInfo != null && comInfo.Count > 0)
				{
					return true;
				}
			}
			while (running && !quit);
			return false;
		});
		if (base.TimeoutMilliseconds == 0)
		{
			base.TimeoutMilliseconds = 30000;
		}
		bool num = task.Wait(base.TimeoutMilliseconds);
		running = false;
		base.Recipe.UcDevice.MessageBox.Close(true);
		Result result = ((num && task.Result) ? Result.PASSED : Result.FIND_PNPDEVICE_FAILED);
		if (quit)
		{
			result = Result.MANUAL_QUIT;
		}
		if (result == Result.PASSED && base.Info.Args.ExPromptText != null)
		{
			Task.Run(delegate
			{
				dynamic val3 = base.Info.Args.ExImage?.ToString();
				dynamic val4 = base.Info.Args.ExPromptText.ToString();
				base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, val4, val3, milliseconds: 5000);
			})?.Wait();
		}
		base.Log.AddResult(this, result, (result == Result.PASSED) ? null : "find pnp device failed");
	}
}
