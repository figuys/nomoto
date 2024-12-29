using System.Collections.Generic;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

public abstract class WebService
{
	private string TAG => GetType().FullName;

	public string Url { get; set; }

	public Login Login { get; set; }

	public abstract dynamic Invoke(dynamic request);

	protected void SentRequest(string request)
	{
		foreach (string item in new List<string> { "<d3p1:id>", "<d3p1:pw>" })
		{
			int num = request.IndexOf(item);
			if (num >= 0)
			{
				int startIndex = request.IndexOf('<', num + item.Length);
				request = request.Substring(0, num + item.Length) + "********" + request.Substring(startIndex);
			}
		}
		string arg = $"{GetType().Name} request";
		LogHelper.LogInstance.Debug("Sent request to " + Url);
		LogHelper.LogInstance.Debug($"{arg} sent:\n{request}");
	}

	protected void ReceivedReply(string reply)
	{
		string arg = $"{GetType().Name} reply";
		LogHelper.LogInstance.Debug("Received reply from " + Url);
		LogHelper.LogInstance.Debug($"{arg} received:\n{reply}");
	}
}
