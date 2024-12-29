using System;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class ServerPicGroupInfo
{
	private DateTime? date;

	public DateTime? Date
	{
		get
		{
			if (string.IsNullOrEmpty(GroupKey))
			{
				return null;
			}
			if (date.HasValue)
			{
				return date;
			}
			DateTime result = DateTime.Now;
			if (DateTime.TryParse(GroupKey, out result))
			{
				date = result;
			}
			return date;
		}
	}

	public int Count { get; set; }

	public string GroupKey { get; set; }
}
