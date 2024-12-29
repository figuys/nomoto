using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class WaitConnectByFastboot : BaseStep
{
	public override void Run()
	{
		Result result = Result.PASSED;
		if (base.TimeoutMilliseconds <= 0)
		{
			base.TimeoutMilliseconds = 20000;
		}
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		Task<List<string>> task = ComportMonitorTask(cancellationTokenSource);
		string response = null;
		bool flag = false;
		if ((base.Recipe.Device != null) ? (HostProxy.deviceManager.ConntectedDevices.Count((DeviceEx n) => n.Identifer == base.Recipe.Device.Identifer) == 0) : (GetDevice(ConnectType.Fastboot) == null))
		{
			int num;
			do
			{
				Task<bool?> msgbxTask = base.Recipe.UcDevice.MessageBox.WaitByFastboot(base.Log.UseCase, base.Recipe.Resources.Get(RecipeResources.ModelName));
				num = FindDevice(msgbxTask);
				if (num == -1)
				{
					break;
				}
				base.Recipe.UcDevice.MessageBox.Close(true);
				if (num == 1)
				{
					break;
				}
				if ((Retry > 0 && base.Info.Args.RetryPromptText != null) && ((!base.Recipe.UcDevice.MessageBox.Show(base.Name, base.Info.Args.RetryPromptText.ToString()).Wait(base.TimeoutMilliseconds)) ? true : false))
				{
					num = 0;
					base.Recipe.UcDevice.MessageBox.Close(true);
					break;
				}
			}
			while (Retry-- > 0);
			switch (num)
			{
			case -1:
				result = Result.MANUAL_QUIT;
				response = "customer closes the connection pop-up window";
				break;
			case 1:
				result = Result.PASSED;
				break;
			default:
				result = Result.FASTBOOT_CONNECT_FAILED;
				response = "connect timeout";
				break;
			}
		}
		while (task.Status != TaskStatus.Running)
		{
		}
		cancellationTokenSource.Cancel(throwOnFirstException: false);
		if (task.Result.Count > 0)
		{
			base.Log.AddLog(string.Join("\r\n", task.Result), upload: true);
		}
		base.Log.AddResult(this, result, response);
	}

	protected int FindDevice(Task<bool?> msgbxTask)
	{
		DateTime now = DateTime.Now;
		do
		{
			if (msgbxTask.IsCompleted)
			{
				return -1;
			}
			if (GetDevice(ConnectType.Fastboot) != null)
			{
				return 1;
			}
			Thread.Sleep(500);
		}
		while (DateTime.Now.Subtract(now).TotalMilliseconds < (double)base.TimeoutMilliseconds);
		return 0;
	}
}
