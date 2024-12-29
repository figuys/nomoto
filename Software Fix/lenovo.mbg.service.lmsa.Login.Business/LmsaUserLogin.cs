using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.Login.Protocol;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class LmsaUserLogin : IUserLoginHandler
{
	public ResponseData<LoggingInResponseData> Login(UserLoginFormData data)
	{
		ResponseData<LoggingInResponseData> responseData = null;
		LogHelper.LogInstance.Debug("lmsa user login start");
		if (!string.IsNullOrEmpty(data.UserData))
		{
			if (JsonConvert.DeserializeObject(data.UserData, typeof(LmsaUserLoginFormData)) is LmsaUserLoginFormData data2)
			{
				responseData = WebServiceProxy.SingleInstance.Login(data2);
			}
			LogHelper.LogInstance.Debug("lmsa user login response: " + JsonHelper.SerializeObject2Json(responseData));
		}
		LogHelper.LogInstance.Debug("lmsa user login end");
		return responseData;
	}

	public void Logout(Action AfterLogoutCall)
	{
	}
}
