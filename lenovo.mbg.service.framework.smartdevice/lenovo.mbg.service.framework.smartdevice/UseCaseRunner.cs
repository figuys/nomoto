using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.framework.smartdevice;

public class UseCaseRunner
{
	public static ConcurrentQueue<UseCaseDevice> Queues = new ConcurrentQueue<UseCaseDevice>();

	public static Dictionary<string, ResultLogger> Loggers = new Dictionary<string, ResultLogger>();

	public static void Run(UseCase useCase, UseCaseDevice device)
	{
		device.StartTime = DateTime.Now.ToString("MM/dd HH:mm:ss");
		ResultLogger resultLogger = new ResultLogger(device)
		{
			UseCase = useCase
		};
		Loggers.Add(resultLogger.ClassGuid, resultLogger);
		device.Log = resultLogger;
		device.Resources.Log = device.Log;
		device.EventHandle = new AutoResetEvent(initialState: false);
		device.Log.AddLog($"***********device: {device.Id}, usecase: {useCase}, start: {device.StartTime}, logid: {resultLogger.ClassGuid}***********", upload: true);
		device.Log.AddLog(device.ToString(), upload: true);
		foreach (KeyValuePair<string, string> resource in device.Resources.Resources)
		{
			device.Log.AddLog("recipe resource: " + resource.Key + " = " + resource.Value);
		}
		Task<object> task = Task.Run(delegate
		{
			string text = device.Resources.Get(RecipeResources.RecipeUrl);
			device.Log.AddLog("Recipe: " + text, upload: true);
			text = (text.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? text : ("http://" + text));
			int num = 2;
			string json = null;
			do
			{
				ResponseModel<string> responseModel = WebApiHttpRequest.RequestBase(text, null, null, HttpMethod.GET);
				if (!string.IsNullOrEmpty(responseModel.content))
				{
					json = responseModel.content;
					break;
				}
				Thread.Sleep(new Random().Next(1000));
			}
			while (num-- > 0);
			return (dynamic)JsonHelper.DeserializeJson2Object(json);
		});
		if ((dynamic)task.Result == null)
		{
			device.Log.AddLog("the recipe content is null", upload: true);
			device.MessageBox.Show("K0181", "K1425").Wait();
			device.Log.NotifyAsync(RecipeMessageType.UNDO, default(RecipeMessage));
			return;
		}
		if (useCase == UseCase.LMSA_Read_Fastboot)
		{
			ExecuteRecipeTask(device, task);
			return;
		}
		lock (Queues)
		{
			IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
			if (conntectedDevices.Count((DeviceEx n) => n.ConnectType != ConnectType.Wifi && n.WorkType == DeviceWorkType.None) > 0 && (device.Device == null || conntectedDevices.Count((DeviceEx n) => n.Identifer == device.Device.Identifer) == 0))
			{
				device.MessageBox.Show("K0711", "K1651").Wait();
			}
			ExecuteRecipeTask(device, task);
		}
	}

	protected static void ExecuteRecipeTask(UseCaseDevice device, Task<dynamic> task)
	{
		Task.Run(delegate
		{
			RecipeInfo recipeInfo = new RecipeInfo();
			recipeInfo.Load(task.Result);
			Recipe recipe = new Recipe(device);
			recipe.Load(recipeInfo);
			recipe.Run();
		});
		device.EventHandle.WaitOne();
	}
}
