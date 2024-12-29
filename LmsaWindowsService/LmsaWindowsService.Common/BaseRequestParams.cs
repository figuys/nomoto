using lenovo.mbg.service.common.utilities;

namespace LmsaWindowsService.Common;

public class BaseRequestParams
{
	public string token
	{
		get
		{
			return Context.TOKEN;
		}
		set
		{
			Context.TOKEN = value;
		}
	}

	public string guid
	{
		get
		{
			return Context.GUID;
		}
		set
		{
			Context.GUID = value;
		}
	}

	public override string ToString()
	{
		return JsonHelper.SerializeObject2Json(this);
	}
}
