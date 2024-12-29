using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class ApiService : ApiBaseService
{
	public object RequestContent(string url, object parameter, int tryCount = 3, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool author = true, bool failedSave = false)
	{
		return RequestBase(url, parameter, tryCount, headers, method, contentType, author, failedSave).content;
	}

	public T RequestContent<T>(string url, object parameter, int tryCount = 3, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool author = true, bool failedSave = false) where T : class, new()
	{
		ResponseModel<object> responseModel = RequestBase(url, parameter, tryCount, headers, method, contentType, author, failedSave);
		if (responseModel.content != null)
		{
			return JsonHelper.DeserializeJson2Object<T>(responseModel.content.ToString());
		}
		return null;
	}

	public ResponseModel<T> Request<T>(string url, object parameter, int tryCount = 3, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool author = true, bool failedSave = false) where T : class, new()
	{
		ResponseModel<object> responseModel = RequestBase(url, parameter, tryCount, headers, method, contentType, author, failedSave);
		ResponseModel<T> responseModel2 = new ResponseModel<T>
		{
			code = responseModel.code,
			desc = responseModel.desc,
			success = responseModel.success
		};
		if (responseModel.content != null)
		{
			responseModel2.content = JsonHelper.DeserializeJson2Object<T>(responseModel.content.ToString());
		}
		return responseModel2;
	}
}
