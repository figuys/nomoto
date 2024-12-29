using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.common.webservices.WebApiServices;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Proxy;

public class UpdateVersionService : ApiService
{
	public ToolVersionModel GetClientVersion(object aparams)
	{
		return RequestContent<ToolVersionModel>(WebApiUrl.CLIENT_VERSION, aparams);
	}
}
