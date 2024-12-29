using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

[Serializable]
public class RequestModel : BaseRequestModel
{
	public Dictionary<string, string> client { get; set; }

	public object dparams { get; set; }

	public string language { get; set; }

	public string windowsInfo { get; set; }

	public RequestModel(object aparams)
	{
		language = WebApiContext.LANGUAGE;
		windowsInfo = WebApiContext.WINDOWS_VERSION;
		client = new Dictionary<string, string> { 
		{
			"version",
			WebApiContext.CLIENT_VERSION
		} };
		dparams = aparams;
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
