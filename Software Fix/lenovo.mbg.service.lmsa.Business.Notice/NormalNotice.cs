using System.Collections.Generic;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.Business.Notice;

public class NormalNotice : INoticeSource
{
	public string NoticeType { get; set; }

	public NormalNotice()
	{
		NoticeType = "Normal";
	}

	public List<NoticeInfo> Filter(List<NoticeInfo> source)
	{
		return source.Where((NoticeInfo m) => NoticeType.Equals(m.type)).ToList();
	}

	public List<NoticeInfo> GetNoticesAsync()
	{
		return AppContext.WebApi.RequestContent<List<NoticeInfo>>(WebApiUrl.NOTICE_URL, null);
	}

	public void Show(NoticeInfo notice)
	{
		if (notice != null)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				ApplcationClass.ApplcationStartWindow.ShowMessage(notice.noticeTitle, notice.noticeContent, "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
			});
		}
	}
}
