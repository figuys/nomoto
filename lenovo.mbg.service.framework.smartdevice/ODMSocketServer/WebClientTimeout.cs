using System;
using System.Net;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

public class WebClientTimeout : WebClient
{
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100.0);

	protected override WebRequest GetWebRequest(Uri uri)
	{
		WebRequest webRequest = base.GetWebRequest(uri);
		webRequest.Timeout = (int)Timeout.TotalMilliseconds;
		return webRequest;
	}
}
