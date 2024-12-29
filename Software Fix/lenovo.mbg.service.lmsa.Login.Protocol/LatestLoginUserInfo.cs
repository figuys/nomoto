using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.Login.Protocol;

public class LatestLoginUserInfo
{
	public static string TAG => "LatestLoginUserInfo";

	public string UserName { get; set; }

	public string Password { get; set; }

	public string UserID { get; set; }

	public string email { get; set; }

	public string phone { get; set; }

	public Dictionary<string, object> config { get; set; }

	public UserLoginFormData LoginFormData { get; set; }
}
