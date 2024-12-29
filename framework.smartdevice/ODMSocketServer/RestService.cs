using System;
using System.Net;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

public abstract class RestService : WebService
{
	private string TAG => GetType().FullName;

	public TimeSpan Timeout { get; protected set; } = TimeSpan.FromSeconds(100.0);

	protected virtual string BasicAuthentication => null;

	public string OAuth { get; set; }

	protected abstract NetworkCredential Credential { get; }

	protected dynamic SendContent(string input, string contentType)
	{
		using WebClientTimeout webClientTimeout = new WebClientTimeout();
		webClientTimeout.Timeout = Timeout;
		if (BasicAuthentication == null)
		{
			webClientTimeout.Credentials = Credential;
		}
		else
		{
			webClientTimeout.Headers[HttpRequestHeader.Authorization] = BasicAuthentication;
		}
		SentRequest(input);
		webClientTimeout.Headers[HttpRequestHeader.ContentType] = contentType;
		string text = webClientTimeout.UploadString(base.Url, "POST", input);
		ReceivedReply(text);
		return ParseResponse(text);
	}

	protected dynamic SendRequest(dynamic input)
	{
		string input2 = JsonConvert.SerializeObject(input, Formatting.Indented);
		return SendContent(input2, "application/json");
	}

	protected dynamic SendForm(dynamic input)
	{
		string text = string.Empty;
		foreach (dynamic item in input)
		{
			if (text != string.Empty)
			{
				text += "&";
			}
			string arg = item.Key.ToString();
			string arg2 = item.Value.ToString();
			text += $"{arg}={arg2}";
		}
		return SendContent(text, "application/x-www-form-urlencoded");
	}

	protected dynamic SendGet(string urlArgs)
	{
		using WebClientTimeout webClientTimeout = new WebClientTimeout();
		webClientTimeout.Timeout = Timeout;
		webClientTimeout.Credentials = Credential;
		SentRequest(urlArgs);
		string text = webClientTimeout.DownloadString(base.Url + urlArgs);
		ReceivedReply(text);
		return ParseResponse(text);
	}

	protected dynamic ParseResponse(string responseContent)
	{
		return JsonConvert.DeserializeObject(responseContent);
	}
}
