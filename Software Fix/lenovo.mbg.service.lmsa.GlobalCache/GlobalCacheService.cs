using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.pipes;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Services;

namespace lenovo.mbg.service.lmsa.GlobalCache;

public class GlobalCacheService : IGlobalCache
{
	private readonly ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();

	public object Get(string key)
	{
		object value;
		if (cache.ContainsKey(key))
		{
			cache.TryGetValue(key, out value);
			return value;
		}
		Init(key);
		cache.TryGetValue(key, out value);
		return value;
	}

	public object AddOrUpdate(string key, object value)
	{
		object result = cache.AddOrUpdate(key, value, (string k, object old) => value);
		if (key == "BackupRestoreHasFailed")
		{
			Task.Run(delegate
			{
				try
				{
					using PipeClientService pipeClientService = new PipeClientService();
					Dictionary<string, object> dictionary = HostProxy.User.user.Config;
					if (dictionary == null)
					{
						dictionary = new Dictionary<string, object>();
					}
					dictionary.Add(key, value);
					pipeClientService.Create(5000);
					pipeClientService.Send(PipeMessage.LMSA_DATA, JsonHelper.SerializeObject2Json(dictionary));
				}
				catch
				{
					LogHelper.LogInstance.Debug("send backuprestore failed pipe message failed");
				}
			});
		}
		return result;
	}

	private void Init(string key)
	{
		if (key == "countrySupportedMoliLenaList")
		{
			LoadMoliLenaConfig(key);
		}
	}

	private void LoadMoliLenaConfig(string key)
	{
		var aparams = new
		{
			country = GlobalFun.GetRegionInfo().TwoLetterISORegionName
		};
		ResponseModel<object> responseModel = AppContext.WebApi.RequestBase(WebApiUrl.MOLI_INFO, aparams);
		if (responseModel.code == "0000")
		{
			AddOrUpdate(key, responseModel.content);
		}
	}
}
