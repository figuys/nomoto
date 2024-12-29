using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

public class BaseRequestModel
{
	public override string ToString()
	{
		return JsonHelper.SerializeObject2Json(this);
	}
}
