using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService.PipeWorkers;

public class PipeLmsaData : IPipeMessageWorker
{
	public static Dictionary<string, object> DataDic = new Dictionary<string, object>();

	public void Do(object data)
	{
		Dictionary<string, object> dictionary = JsonHelper.DeserializeJson2Object<Dictionary<string, object>>(data?.ToString());
		if (dictionary == null || dictionary.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			DataDic[item.Key] = item.Value;
		}
	}
}
