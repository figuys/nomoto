using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

internal class RsaService
{
	public static string InitPublicKey()
	{
		new RSAKey();
		_ = string.Empty;
		ResponseModel<object> responseModel = WebApiHttpRequest.Request(WebApiUrl.GET_PUBLIC_KEY, null, null, HttpMethod.POST, "application/json", addAuthorizationHeader: true);
		if (responseModel.success && !string.IsNullOrEmpty(responseModel.desc) && RsaHelper.RSAPublicKeyJava2DotNet(responseModel.desc, out var key))
		{
			return key;
		}
		return null;
	}
}
