using System.Collections.Generic;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.Business;

public class RsaServiceHelper
{
	public static void PostUserBehavior()
	{
		List<BehaviorModel> all = HostProxy.BehaviorService.GetAll();
		if (all != null && all.Count > 0)
		{
			AppContext.WebApi.RequestContent(WebApiUrl.USER_BEHAVIOR_COLLECTION, all);
		}
	}
}
