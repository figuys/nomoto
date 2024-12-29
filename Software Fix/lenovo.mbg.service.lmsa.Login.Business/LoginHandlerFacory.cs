using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class LoginHandlerFacory
{
	public static IUserLoginHandler CreateLoginHandler(string userSource)
	{
		if (!(userSource == "lenovoId"))
		{
			if (userSource == "lmsa")
			{
				LogHelper.LogInstance.Info("Create lmsa login handler");
				return new LmsaUserLogin();
			}
			LogHelper.LogInstance.Info("Create guest login handler");
			return new GuestLogin();
		}
		LogHelper.LogInstance.Info("Create lenovo id login handler");
		return new LenovoIdUserLogin();
	}
}
