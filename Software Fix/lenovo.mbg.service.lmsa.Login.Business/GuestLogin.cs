using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.Login.Protocol;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class GuestLogin : IUserLoginHandler
{
	public ResponseData<LoggingInResponseData> Login(UserLoginFormData data)
	{
		LogHelper.LogInstance.Debug("guest login start");
		JObject jObject = new JObject();
		data.UserSource = "guest";
		jObject.Add("accountId", data.UserData);
		ResponseData<LoggingInResponseData> responseData = WebServiceProxy.SingleInstance.GuestLogin(jObject);
		LogHelper.LogInstance.Debug("guest login response: " + JsonHelper.SerializeObject2Json(responseData));
		LogHelper.LogInstance.Debug("guest login end");
		return responseData;
	}

	public void Logout(Action AfterLogoutCall)
	{
	}
}
