using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.Business.Notice;

public interface INoticeSource
{
	string NoticeType { get; set; }

	List<NoticeInfo> GetNoticesAsync();

	void Show(NoticeInfo notice);

	List<NoticeInfo> Filter(List<NoticeInfo> source);
}
