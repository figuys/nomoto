using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.smartdevice;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class RescueSubmitManager
{
	private List<Tuple<string, RescueCollectionModel, Dictionary<string, string>>> SubmitTaskList = new List<Tuple<string, RescueCollectionModel, Dictionary<string, string>>>();

	private Dictionary<string, Task> SubmitTasks = new Dictionary<string, Task>();

	private Dictionary<string, List<ResultLogger>> LogDic = new Dictionary<string, List<ResultLogger>>();

	public static RescueSubmitManager Instance { get; } = new RescueSubmitManager();

	public void CreateSubmitTask(string resultLoggerId, RescueCollectionModel data, BusinessType businessType, BusinessData businessData, string recipeName, Dictionary<string, string> degradeInfos)
	{
		Task value = new Task(delegate(object o)
		{
			Tuple<string, RescueCollectionModel, BusinessType, BusinessData, string, Dictionary<string, string>> tuple = o as Tuple<string, RescueCollectionModel, BusinessType, BusinessData, string, Dictionary<string, string>>;
			string logId = tuple.Item1;
			RescueCollectionModel item = tuple.Item2;
			Dictionary<string, string> degradeinfo = tuple.Item6;
			BusinessType item2 = tuple.Item3;
			BusinessData item3 = tuple.Item4;
			string item4 = tuple.Item5;
			double totalMilliseconds = DateTime.Parse(item.rescueTime).Subtract(DateTime.Parse(item.startRescueTime)).TotalMilliseconds;
			int rescueResult = item.rescueResult;
			List<ResultLogger> list = new List<ResultLogger>();
			List<ResultLogger> list2 = LogDic[logId];
			if (list2.Count == 1)
			{
				list.Add(list2.First());
			}
			else
			{
				long minStartTimestamp = list2.Min((ResultLogger n) => n.StartTimestamp);
				list = UseCaseRunner.Loggers.Values.Where((ResultLogger n) => n.EndTimestamp < minStartTimestamp).ToList();
			}
			if (data.rescueResult == 0)
			{
				List<ResultLogger> list3 = list2.Where((ResultLogger n) => n.ClassGuid != logId).ToList();
				if (list3 != null && list3.Count > 0)
				{
					foreach (ResultLogger item5 in list3)
					{
						item.errorMsg = item.errorMsg + Environment.NewLine + item5.UploadLogs;
					}
				}
			}
			LogDic.Remove(logId);
			if (list.Count > 0)
			{
				foreach (ResultLogger item6 in list)
				{
					item6.LogClear();
					UseCaseRunner.Loggers.Remove(item6.ClassGuid);
				}
			}
			ResponseModel<object> responseModel = FlashContext.SingleInstance.service.RequestBase(WebServicesContext.UPLOAD_WHEN_FLASH_FINISHED, item, 3, null, HttpMethod.POST, "application/json", author: true, failedSave: true);
			LogHelper.LogInstance.WriteLogForUser(JsonHelper.SerializeObject2Json(item), item.rescueResult);
			long flashId = 0L;
			if (responseModel.content != null)
			{
				long.TryParse(responseModel.content.ToString(), out flashId);
			}
			if (flashId > 0 && degradeinfo != null)
			{
				degradeinfo.Add("rescueFlashId", flashId.ToString());
				Task.Run(() => FlashContext.SingleInstance.service.RequestContent(WebServicesContext.COLLECTION_DOWNGRADE_INFO, degradeinfo));
			}
			string directory = Path.Combine(Path.GetTempPath(), "lmsatemp");
			List<string> logfiles = GlobalFun.GetAllFiles(directory);
			if (logfiles != null && logfiles.Count > 0)
			{
				Task.Run(() => FlashContext.SingleInstance.service.UploadAsync(WebApiUrl.UPLOAD_RESCUE_TOOL_LOG, logfiles, new Dictionary<string, string> { 
				{
					"flashId",
					flashId.ToString()
				} })).ContinueWith((Task<bool> s) => GlobalFun.DeleteDirectoryEx(directory));
			}
			BusinessStatus status = BusinessStatus.SUCCESS;
			switch (rescueResult)
			{
			case 2:
				status = BusinessStatus.QUIT;
				break;
			default:
				status = BusinessStatus.FALIED;
				break;
			case 1:
				break;
			}
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(JsonHelper.SerializeObject2JsonExceptNull(item));
			jObject["flashId"] = flashId;
			jObject["recipeName"] = item4;
			HostProxy.BehaviorService.Collect(item2, item3.Update((long)totalMilliseconds, status, item.modelName, jObject));
		}, new Tuple<string, RescueCollectionModel, BusinessType, BusinessData, string, Dictionary<string, string>>(resultLoggerId, data, businessType, businessData, recipeName, degradeInfos));
		SubmitTasks.Add(resultLoggerId, value);
		ScanTask();
	}

	private void ScanTask()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Task> submitTask in SubmitTasks)
		{
			ResultLogger logger = UseCaseRunner.Loggers[submitTask.Key];
			List<ResultLogger> list2 = UseCaseRunner.Loggers.Values.Where((ResultLogger n) => HasInterval(logger.StartTimestamp, logger.EndTimestamp, n.StartTimestamp, n.EndTimestamp)).ToList();
			bool flag = list2.Exists((ResultLogger n) => n.EndTimestamp == long.MaxValue);
			if (logger.OverallResult != 0 || !flag)
			{
				LogDic.Add(submitTask.Key, list2);
				list.Add(submitTask.Key);
				submitTask.Value.Start();
			}
		}
		foreach (string item in list)
		{
			SubmitTasks.Remove(item);
		}
	}

	private bool HasInterval(long start1, long end1, long start2, long end2)
	{
		long num = Math.Max(start1, start2);
		long num2 = Math.Min(end1, end2);
		return num < num2;
	}
}
