using System;
using lenovo.mbg.service.lmsa.Login.Protocol;

namespace lenovo.mbg.service.lmsa.Login.Business;

public interface IUserLoginHandler
{
	ResponseData<LoggingInResponseData> Login(UserLoginFormData data);

	void Logout(Action AfterLogoutCall);
}
