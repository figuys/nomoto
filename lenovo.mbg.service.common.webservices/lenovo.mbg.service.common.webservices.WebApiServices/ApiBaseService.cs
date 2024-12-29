using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class ApiBaseService
{
	public virtual ResponseModel<object> RequestBase(string url, object aparams, int tryCount = 3, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool author = true, bool failedSave = false)
	{
		ResponseModel<object> responseModel;
		do
		{
			responseModel = func(url, aparams);
			if (responseModel.success)
			{
				break;
			}
			Thread.Sleep(new Random().Next(1000));
		}
		while (--tryCount > 0);
		if (failedSave && !responseModel.success)
		{
			AsyncSaveData(url, aparams);
		}
		return responseModel;
		ResponseModel<object> func(string uri, object data)
		{
			return WebApiHttpRequest.Request(uri, (method == HttpMethod.POST) ? new RequestModel(data).ToString() : null, headers, method, contentType, author);
		}
	}

	public virtual async Task<bool> UploadAsync(string url, List<string> files, Dictionary<string, string> headers, bool extraHeader = false, bool author = true)
	{
		return await WebApiHttpRequest.UploadAsync(url, files, headers, extraHeader, author);
	}

	private void AsyncSaveData(string url, object data)
	{
		Task.Factory.StartNew(delegate
		{
			JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.UserRequestRecordsFile, "$.content", isDateAsStr: true);
			if (jArray == null)
			{
				jArray = JArray.Parse("[]");
			}
			JToken jToken = jArray.FirstOrDefault((JToken n) => n.Value<string>("url") == url);
			if (jToken == null)
			{
				jToken = new JObject
				{
					{ "url", url },
					{
						"datas",
						JArray.Parse("[]")
					}
				};
				jArray.Add(jToken);
			}
			JArray jArray2 = jToken.SelectToken("$.datas") as JArray;
			if (jArray2 == null)
			{
				jArray2 = (JArray)(jToken["datas"] = JArray.Parse("[]"));
			}
			try
			{
				jArray2.Add(JToken.FromObject(data));
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("BaseService.AsyncSaveData Exception: " + ex.ToString());
			}
			FileHelper.WriteJsonWithAesEncrypt(Configurations.UserRequestRecordsFile, "content", jArray);
		});
	}
}
