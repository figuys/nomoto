using System;

namespace lenovo.mbg.service.lmsa.Business.Notice;

public class Notice
{
	public int id { get; set; }

	public string noticeTitle { get; set; }

	public string noticeContent { get; set; }

	public DateTime modifyDate { get; set; }

	public bool isDeleted { get; set; }

	public bool isChecked { get; set; }

	public string type { get; set; }
}
