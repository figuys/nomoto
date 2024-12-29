using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Feedback.Model;
using lenovo.mbg.service.lmsa.Feedback.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.Feedback.Business;

public class FeedBackBLL
{
	private WebService mService = new WebService();

	public async Task<List<FeedBackNodeModel>> GetFeedbackIdAndTimeListAsync()
	{
		return await mService.GetFeedbackIdAndTimeListAsync();
	}

	public async Task<FeedBackNodeModel> GetFeedbackDetailAsync(long feedbackId)
	{
		return await mService.GetFeedbackDetailAsync(feedbackId);
	}

	public async Task<bool> SubmitFeedbackAsync(FeedbackSubmitModel feedback, List<string> files, bool _isLogin = true)
	{
		return await mService.SubmitFeedbackAsync(feedback, files, _isLogin);
	}

	public async Task<string> GetUrlWithToken(string originalUrl)
	{
		return await mService.GetUrlWithToken(originalUrl);
	}

	public async Task<bool> SubmitReplyIsHelpfull(long? replyId, int? helpfullCode)
	{
		return await mService.SubmitReplyIsHelpfull(replyId, helpfullCode);
	}

	public List<string> GetLogFileCopy()
	{
		List<string> list = new List<string>();
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
		string text = DateTime.Now.ToString("yyyy-MM-dd");
		string encryptFile = Path.Combine(path, text + ".log.dpapi");
		LogAesDecrypt logAesDecrypt = new LogAesDecrypt();
		string text2 = Path.Combine(path, text + ".decrpyt.log");
		if (logAesDecrypt.Decrypt2File(encryptFile, text2))
		{
			list.Add(text2);
		}
		string text3 = Path.Combine(Path.GetTempPath(), "lmsatemp");
		if (!Directory.Exists(text3))
		{
			Directory.CreateDirectory(text3);
		}
		else
		{
			GlobalFun.DeleteFileInDirectory(text3);
		}
		string chromiumLogFilePath = Configurations.ChromiumLogFilePath;
		if (File.Exists(chromiumLogFilePath))
		{
			string text4 = Path.Combine(text3, DateTime.Now.ToString("yyyy-MM-dd") + "-browser.log");
			File.Copy(chromiumLogFilePath, text4, overwrite: true);
			list.Add(text4);
		}
		return list;
	}

	public List<FeedbackItemViewModel> CreateFeedbackItemViewModels(FeedBackNodeModel feedback)
	{
		Queue<FeedBackNodeModel> queue = new Queue<FeedBackNodeModel>();
		List<FeedbackItemViewModel> list = new List<FeedbackItemViewModel>();
		string text = "K0732";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			text = HostProxy.LanguageService.Translate(text);
		}
		bool flag = true;
		queue.Enqueue(feedback);
		while (queue.Count != 0)
		{
			FeedBackNodeModel feedBackNodeModel = queue.Dequeue();
			if (feedBackNodeModel.Children != null)
			{
				IOrderedEnumerable<FeedBackNodeModel> orderedEnumerable = feedBackNodeModel.Children.OrderBy((FeedBackNodeModel m) => m.Date);
				foreach (FeedBackNodeModel item in orderedEnumerable)
				{
					queue.Enqueue(item);
				}
			}
			FeedbackItemViewModel feedbackItemViewModel = new FeedbackItemViewModel
			{
				Title = feedBackNodeModel.Title,
				Type = feedBackNodeModel.Type,
				Date = DateTime.SpecifyKind(feedBackNodeModel.Date, DateTimeKind.Utc).ToLocalTime(),
				Id = feedBackNodeModel.Id,
				HelpfulCode = ((feedBackNodeModel.HelpfulCode.HasValue && (feedBackNodeModel.HelpfulCode.Value == 1 || feedBackNodeModel.HelpfulCode.Value == -1)) ? null : feedBackNodeModel.HelpfulCode)
			};
			LogHelper.LogInstance.Info($"Feedback date convert info:[Title{feedbackItemViewModel.Title},SData:{feedBackNodeModel.Date},LData:{feedbackItemViewModel.Date}]");
			List<FeedbackSubContentItemViewModel> list2 = null;
			if ("Q".Equals(feedbackItemViewModel.Type))
			{
				list2 = new List<FeedbackSubContentItemViewModel>
				{
					new FeedbackSubContentItemViewModel
					{
						DataType = "other",
						Content = feedBackNodeModel.Content?.Replace("<br>", "")
					}
				};
				if (flag)
				{
					flag = false;
					feedbackItemViewModel.Title = text;
					text = HostProxy.LanguageService.Translate("K0731");
				}
				else
				{
					feedbackItemViewModel.Title = text;
				}
			}
			else if (feedBackNodeModel.ContentItems != null && feedBackNodeModel.ContentItems.Count > 0)
			{
				list2 = new List<FeedbackSubContentItemViewModel>();
				foreach (ContentItem item2 in feedBackNodeModel.ContentItems.OrderBy((ContentItem m) => m.Sort))
				{
					list2.Add(new FeedbackSubContentItemViewModel
					{
						DataType = item2.DataType,
						Content = item2.Content?.Replace("<br>", "")
					});
				}
			}
			feedbackItemViewModel.FeedbackContentItmes = list2;
			list.Insert(0, feedbackItemViewModel);
		}
		return list;
	}

	public bool HaveReplyFromService(FeedBackNodeModel feedback)
	{
		if (feedback == null)
		{
			return false;
		}
		FeedBackNodeModel feedBackNodeModel = feedback;
		while (feedBackNodeModel != null && feedBackNodeModel.Children != null && feedBackNodeModel.Children.Count > 0)
		{
			FeedBackNodeModel feedBackNodeModel2 = feedBackNodeModel.Children.FirstOrDefault((FeedBackNodeModel m) => m.Children != null && m.Children.Count > 0);
			feedBackNodeModel = ((feedBackNodeModel2 == null) ? feedBackNodeModel.Children.FirstOrDefault() : feedBackNodeModel2);
		}
		return "A".Equals(feedBackNodeModel.Type);
	}
}
