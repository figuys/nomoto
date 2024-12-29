using System.Collections.Generic;
using System.Dynamic;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.lmsa.Login.Protocol;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Business;

public class WebServiceProxy
{
	private static WebServiceProxy _webServiceProxy;

	public static WebServiceProxy SingleInstance
	{
		get
		{
			if (_webServiceProxy == null)
			{
				_webServiceProxy = new WebServiceProxy();
			}
			return _webServiceProxy;
		}
	}

	public List<NoticeInfo> GetNotice()
	{
		return AppContext.WebApi.RequestContent<List<NoticeInfo>>(WebApiUrl.NOTICE_BROADCAST_URL, null);
	}

	public ResponseData<LoggingInResponseData> Login(LmsaUserLoginFormData data)
	{
		return Request<LmsaUserLoginFormData, LoggingInResponseData>(data, WebApiUrl.USER_RECORD_LOGIN);
	}

	public ResponseData<LoggingInResponseData> GuestLogin(object data)
	{
		return Request<object, LoggingInResponseData>(data, WebApiUrl.USER_GUEST_LOGIN);
	}

	public ResponseData<LoggingInResponseData> PasswordVerify(LmsaUserLoginFormData data)
	{
		return Request<LmsaUserLoginFormData, LoggingInResponseData>(data, WebApiUrl.USER_LOGIN);
	}

	public ResponseData<ChangePasswordResponseData> ChangePassowrd(ChangePasswordFormData data)
	{
		return Request<ChangePasswordFormData, ChangePasswordResponseData>(data, WebApiUrl.USER_CHANGE_PASSWORD);
	}

	public ResponseData<List<UserPermissionResponseData>> GetPermission(object data)
	{
		return Request<object, List<UserPermissionResponseData>>(data, WebApiUrl.PRIV_GET_PRIV_INFO);
	}

	public ResponseData<ForgotPasswordResponseData> ForgotPassowrd(ForgotPasswordFormData data)
	{
		return Request<ForgotPasswordFormData, ForgotPasswordResponseData>(data, WebApiUrl.USER_FORGOT_PASSWORD);
	}

	public ResponseData<LogoutReportResponseData> LogoutReport(LogoutReportFormData data)
	{
		return Request<LogoutReportFormData, LogoutReportResponseData>(data, WebApiUrl.USER_LOGOUT);
	}

	public ResponseData<object> reportConnectedAppType(string appType)
	{
		int num = ("Ma".Equals(appType) ? 1 : 0);
		dynamic val = new ExpandoObject();
		val.appType = num;
		return Request<object, object>(val, WebApiUrl.COLLECTION_ASSISTANTAPP, true);
	}

	private ResponseData<TResponseData> Request<TRequestData, TResponseData>(TRequestData requestData, string url, bool autoSubmitAgainWhenFailed = false)
	{
		ResponseData<TResponseData> responseData = new ResponseData<TResponseData>();
		ResponseModel<object> responseModel = AppContext.WebApi.RequestBase(url, requestData, 3, null, HttpMethod.POST, "application/json", author: true, autoSubmitAgainWhenFailed);
		if (responseModel.success)
		{
			responseData.Code = responseModel.code;
			responseData.Description = responseModel.desc;
		}
		else
		{
			responseData.Code = "1000";
			responseData.Description = "K0039";
		}
		if (!string.IsNullOrEmpty(responseModel.content?.ToString()))
		{
			TResponseData data = JsonConvert.DeserializeObject<TResponseData>(responseModel.content.ToString());
			responseData.Data = data;
		}
		return responseData;
	}
}
