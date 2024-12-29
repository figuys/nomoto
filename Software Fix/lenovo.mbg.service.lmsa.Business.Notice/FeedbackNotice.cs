using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Feedback.Business;
using lenovo.mbg.service.lmsa.Feedback.Model;
using lenovo.mbg.service.lmsa.Feedback.View;
using lenovo.mbg.service.lmsa.Feedback.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.Login.Model;

namespace lenovo.mbg.service.lmsa.Business.Notice;

public class FeedbackNotice : INoticeSource
{
	public class FeedbackNoticeTag
	{
		public string UserId { get; set; }
	}

	private FeedBackBLL bll = new FeedBackBLL();

	private List<FeedBackNodeModel> feedbacks = null;

	public string NoticeType { get; set; }

	public FeedbackNotice()
	{
		NoticeType = "Feedback";
	}

	public async void Show(NoticeInfo notice)
	{
		if (notice == null)
		{
			return;
		}
		FeedBackNodeModel feedback = await bll.GetFeedbackDetailAsync(notice.id);
		if (feedback != null)
		{
			List<FeedbackItemViewModel> viewModels = bll.CreateFeedbackItemViewModels(feedback);
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				FeedbackListViewV6 ui = new FeedbackListViewV6
				{
					DataContext = new FeedbackListViewModelV6
					{
						FeedBack = feedback,
						WinTitle = notice.noticeTitle,
						FeedbackItems = new ObservableCollection<FeedbackItemViewModel>(viewModels)
					}
				};
				ApplcationClass.ApplcationStartWindow.ShowMessage(ui);
			});
		}
	}

	public List<NoticeInfo> GetNoticesAsync()
	{
		OnlineUserInfo currentLoggedInUser = UserService.Single.CurrentLoggedInUser;
		if (currentLoggedInUser == null)
		{
			return null;
		}
		feedbacks = bll.GetFeedbackIdAndTimeListAsync().Result;
		if (feedbacks == null)
		{
			return null;
		}
		List<NoticeInfo> list = new List<NoticeInfo>();
		foreach (FeedBackNodeModel feedback in feedbacks)
		{
			string text = (string.IsNullOrEmpty(feedback.Title) ? string.Empty : feedback.Title);
			text = ((text.Length > 25) ? (text.Substring(0, 25) + "...") : text);
			FeedbackNoticeTag feedbackNoticeTag = new FeedbackNoticeTag();
			feedbackNoticeTag.UserId = currentLoggedInUser.UserId;
			list.Add(new NoticeInfo
			{
				id = feedback.Id.GetValueOrDefault(),
				noticeTitle = text,
				modifyDate = feedback.Date,
				type = NoticeType,
				noticeContent = string.Empty,
				isServerReplay = feedback.IsServerReplay,
				noticeType = 0,
				tag = JsonHelper.SerializeObject2Json(feedbackNoticeTag)
			});
		}
		return list;
	}

	public List<NoticeInfo> Filter(List<NoticeInfo> source)
	{
		string text = UserService.Single.CurrentLoggedInUser?.UserId;
		NoticeInfo noticeInfo = null;
		List<NoticeInfo> list = source.Where((NoticeInfo m) => NoticeType.Equals(m.type)).ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			noticeInfo = list[num];
			if (NoticeType.Equals(noticeInfo.type))
			{
				if (string.IsNullOrEmpty(text))
				{
					list.RemoveAt(num);
				}
				else
				{
					FeedbackNoticeTag feedbackNoticeTag = JsonHelper.DeserializeJson2Object<FeedbackNoticeTag>(noticeInfo.tag);
					if (feedbackNoticeTag == null || !text.Equals(feedbackNoticeTag.UserId))
					{
						list.RemoveAt(num);
					}
				}
			}
		}
		return list;
	}
}
