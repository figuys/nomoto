using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class WaitConnectByAdb : BaseStep
{
	protected bool running = true;

	public override void Run()
	{
		string message = base.Info.Args.PromptText.ToString();
		string image = string.Empty;
		if (base.Info.Args.Image != null)
		{
			image = base.Info.Args.Image.ToString();
		}
		Task<bool?> msgbxTask = base.Recipe.UcDevice.MessageBox.WaitByAdb(base.Info.Name, message, image);
		int num = FindDevice(msgbxTask);
		if (num == -1)
		{
			base.Log.AddResult(this, Result.MANUAL_QUIT, "customer closes the connection pop-up window");
			return;
		}
		base.Recipe.UcDevice.MessageBox.Close(true);
		if (num == 1)
		{
			base.Log.AddResult(this, Result.PASSED);
		}
		else
		{
			base.Log.AddResult(this, Result.ADB_CONNECT_FAILED, "connect timeout");
		}
	}

	protected int FindDevice(Task<bool?> msgbxTask)
	{
		Task<int> task = Task.Run(delegate
		{
			do
			{
				try
				{
					if (GetDevice(ConnectType.Adb, (object d) => !Regex.IsMatch((d as DeviceEx).Identifer, "&|:")) != null)
					{
						return 1;
					}
				}
				catch
				{
				}
				Thread.Sleep(1000);
			}
			while (!msgbxTask.IsCompleted);
			return -1;
		});
		int millisecondsTimeout = ((base.TimeoutMilliseconds <= 0) ? (-1) : base.TimeoutMilliseconds);
		if (!task.Wait(millisecondsTimeout))
		{
			return 0;
		}
		return task.Result;
	}
}
