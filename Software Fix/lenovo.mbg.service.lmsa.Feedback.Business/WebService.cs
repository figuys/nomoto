using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.lmsa.Feedback.Model;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Feedback.Business;

internal class WebService : ApiService
{
	public async Task<List<FeedBackNodeModel>> GetFeedbackIdAndTimeListAsync()
	{
		Task<List<FeedBackNodeModel>> task = new Task<List<FeedBackNodeModel>>(() => RequestContent<List<FeedBackNodeModel>>(WebApiUrl.FEEDBACK_GET_LIST, null));
		task.Start();
		return await task;
	}

	public async Task<FeedBackNodeModel> GetFeedbackDetailAsync(long feedbackId)
	{
		Task<FeedBackNodeModel> task = new Task<FeedBackNodeModel>(delegate
		{
			dynamic val = new ExpandoObject();
			val.feedbackId = feedbackId;
			return RequestContent<FeedBackNodeModel>(WebApiUrl.FEEDBACK_GET_INFO, val);
		});
		task.Start();
		return await task;
	}

	public async Task<string> GetUrlWithToken(string originalUrl)
	{
		Task<string> task = new Task<string>(delegate
		{
			dynamic val = new ExpandoObject();
			val.fileNames = new string[1] { originalUrl };
			List<string> list = RequestContent<List<string>>(WebApiUrl.FEEDBACK_FILE_SINGNATURE, val);
			return (list.Count > 0) ? list[0] : string.Empty;
		});
		task.Start();
		return await task;
	}

	public async Task<bool> SubmitReplyIsHelpfull(long? replyId, int? helpfullCode)
	{
		return await Task.Run(delegate
		{
			JObject aparams = new JObject
			{
				{ "replyId", replyId },
				{ "helpful", helpfullCode }
			};
			ResponseModel<object> responseModel = RequestBase(WebApiUrl.FEEDBACK_GET_HELPFUL, aparams);
			return responseModel.code == "0000";
		});
	}

	public async Task<bool> SubmitFeedbackAsync(FeedbackSubmitModel feedback, List<string> files, bool _isLogin)
	{
		Dictionary<string, string> header = new Dictionary<string, string>();
		header["clientVersion"] = WebApiContext.CLIENT_VERSION;
		header["feedbackContent"] = feedback.FeedbackContent;
		header["sn"] = feedback.SN;
		header["imei"] = feedback.Imei1;
		header["imei2"] = feedback.Imei2;
		header["modelName"] = feedback.ModelName;
		header["marketName"] = feedback.MarketName;
		header["reply"] = feedback.IsReplay.ToString();
		header["userName"] = feedback.UserName;
		header["email"] = feedback.Email;
		header.Add("windowsInfo", WebApiContext.WINDOWS_VERSION);
		if (!string.IsNullOrEmpty(feedback.Module))
		{
			header["feedbackIssueModuleId"] = feedback.Module;
		}
		if (feedback.FeedbackId.HasValue)
		{
			header["feedbackId"] = feedback.FeedbackId.Value.ToString();
		}
		string _feedBackUrl = (_isLogin ? WebApiUrl.FEEDBACK_GET_UPLOAD : WebApiUrl.FEEDBACK_GET_UPLOAD_GUEST);
		return await UploadAsync(_feedBackUrl, files, header);
	}
}
