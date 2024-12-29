using System;
using System.Threading.Tasks;
using System.Timers;
using DotNetBrowser.Wpf;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.pipes;
using lenovo.mbg.service.lmsa.Login.Protocol;
using lenovo.mbg.service.lmsa.Services;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Login.Business;

public class LenovoIdUserLogin : IUserLoginHandler
{
	private static Timer m_timer;

	public ResponseData<LoggingInResponseData> Login(UserLoginFormData userInfo)
	{
		if (string.IsNullOrEmpty(userInfo.UserData))
		{
			return new ResponseData<LoggingInResponseData>
			{
				Code = "FAILED"
			};
		}
		LenovoIdUserLoginFormData lenovoIdUserLoginFormData = JsonConvert.DeserializeObject(userInfo.UserData, typeof(LenovoIdUserLoginFormData)) as LenovoIdUserLoginFormData;
		ResponseModel<LoggingInResponseData> responseModel = AppContext.WebApi.Request<LoggingInResponseData>(WebApiUrl.LENOVOID_LOGIN_CALLBACK, new
		{
			wust = lenovoIdUserLoginFormData.WUST,
			guid = WebApiContext.GUID
		}, 3, null, HttpMethod.POST, "application/json", author: false);
		ResponseData<LoggingInResponseData> responseData = new ResponseData<LoggingInResponseData>
		{
			Code = responseModel.code
		};
		if (responseModel != null && responseModel.code == "0000")
		{
			if (m_timer != null)
			{
				m_timer.Stop();
				m_timer = null;
			}
			m_timer = new Timer(1200000.0);
			m_timer.Elapsed += OnTimedEvent;
			m_timer.Start();
			responseData.Data = responseModel.content;
			LogHelper.LogInstance.Info("get lenovo account info successed");
		}
		else
		{
			responseData.Data = new LoggingInResponseData();
			LogHelper.LogInstance.Info("get lenovo account info failed");
		}
		responseData.Data.IsLenovoId = true;
		return responseData;
	}

	private void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		using PipeClientService pipeClientService = new PipeClientService();
		pipeClientService.Create(5000);
		pipeClientService.Send(PipeMessage.LMSA_DATA, WebApiContext.REQUEST_AUTHOR_HEADERS);
	}

	public void Logout(Action AfterLogoutCall)
	{
		m_timer.Stop();
		m_timer = null;
		string logouturl = "https://passport.lenovo.com/wauthen5/gateway?lenovoid.action=uilogout&lenovoid.realm=lmsaclient";
		BrowserView browserView = new BrowserView();
		Task.Run(delegate
		{
			DotNetBrowserHelper.Instance.LoadUrl(browserView, logouturl, null);
			AfterLogoutCall?.Invoke();
		});
	}
}
