using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices;

public class WebApiHttpRequest
{
	private static long interlocker;

	private static long windowShowTime;

	private static bool windowIsShow;

	public static Func<string, object, object> WebApiCallback { get; set; }

	public static ResponseModel<string> RequestBase(string url, string body, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool addAuthorizationHeader = false)
	{
		HttpWebRequest httpWebRequest = null;
		try
		{
			GC.Collect();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.DefaultConnectionLimit = 200;
			LogHelper.LogInstance.Debug("Request " + url + ", params: " + body);
			httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			httpWebRequest.ServicePoint.ConnectionLimit = 200;
			httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36";
			httpWebRequest.Method = method.ToString();
			httpWebRequest.ContentType = contentType;
			httpWebRequest.KeepAlive = false;
			httpWebRequest.Timeout = 120000;
			httpWebRequest.ReadWriteTimeout = 600000;
			httpWebRequest.ContentLength = 0L;
			httpWebRequest.Headers.Add("Cache-Control", "no-cache");
			httpWebRequest.Headers.Add("Request-Tag: lmsa");
			if (addAuthorizationHeader)
			{
				foreach (KeyValuePair<string, string> rEQUEST_AUTHOR_HEADER in WebApiContext.REQUEST_AUTHOR_HEADERS)
				{
					httpWebRequest.Headers.Add(rEQUEST_AUTHOR_HEADER.Key, rEQUEST_AUTHOR_HEADER.Value);
				}
			}
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					httpWebRequest.Headers.Add(header.Key, header.Value);
				}
			}
			if (!string.IsNullOrEmpty(body))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(body);
				httpWebRequest.ContentLength = bytes.Length;
				using Stream stream = httpWebRequest.GetRequestStream();
				stream.Write(bytes, 0, bytes.Length);
			}
			string text = null;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					string responseHeader = httpWebResponse.GetResponseHeader("Guid");
					string responseHeader2 = httpWebResponse.GetResponseHeader("Authorization");
					if (responseHeader == WebApiContext.GUID && !string.IsNullOrEmpty(responseHeader2))
					{
						WebApiContext.JWT_TOKEN = responseHeader2;
					}
					using Stream stream2 = httpWebResponse.GetResponseStream();
					using MemoryStream memoryStream = new MemoryStream();
					byte[] array = new byte[10240];
					for (int num = stream2.Read(array, 0, array.Length); num > 0; num = stream2.Read(array, 0, array.Length))
					{
						memoryStream.Write(array, 0, num);
					}
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using StreamReader streamReader = new StreamReader(memoryStream);
					text = streamReader.ReadToEnd();
				}
			}
			if (url == WebApiUrl.LENOVOID_LOGIN_CALLBACK)
			{
				LogHelper.LogInstance.AnalyzeUnsafeText(text);
			}
			LogHelper.LogInstance.Debug("Response " + url + ", result: " + text);
			return new ResponseModel<string>
			{
				code = "0000",
				success = true,
				content = text
			};
		}
		catch (WebException ex)
		{
			WebException ex2 = ex;
			WebException ex3 = ex2;
			if (url.Contains(Configurations.BaseHttpUrl))
			{
				LogHelper.LogInstance.Error($"Get Context From Url: [{url}] WebException:{ex3}");
				Task.Delay(new Random().Next(100)).ContinueWith(delegate
				{
					if (Interlocked.Read(ref interlocker) == 0L)
					{
						Interlocked.Exchange(ref interlocker, 1L);
						long num2 = GlobalFun.ToUtcTimeStamp(DateTime.Now);
						if (!windowIsShow && windowShowTime + 60000 < num2)
						{
							windowIsShow = true;
							WebApiCallback?.Invoke("NONETWORK", ex3.Status == WebExceptionStatus.NameResolutionFailure);
							windowShowTime = num2;
							windowIsShow = false;
						}
						Interlocked.Exchange(ref interlocker, 0L);
					}
				});
			}
			else
			{
				LogHelper.LogInstance.Error("Get Context From Third Url: [" + url + "] WebException:" + ex3.Message);
			}
			return new ResponseModel<string>
			{
				code = "ERROR",
				desc = ex3.Message
			};
		}
		catch (Exception ex4)
		{
			LogHelper.LogInstance.Error("Get Context From Url: [" + url + "] Exception:" + ex4.Message);
			return new ResponseModel<string>
			{
				code = "ERROR",
				desc = ex4.Message
			};
		}
		finally
		{
			try
			{
				httpWebRequest?.Abort();
			}
			catch (Exception)
			{
			}
		}
	}

	public static ResponseModel<object> Request(string url, string body, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool addAuthorizationHeader = false)
	{
		ResponseModel<string> responseModel = RequestBase(url, body, headers, method, contentType, addAuthorizationHeader);
		if (responseModel.success)
		{
			ResponseModel<object> responseModel2 = JsonHelper.DeserializeJson2Object<ResponseModel<object>>(responseModel.content);
			if (responseModel2.code == "402")
			{
				Task.Delay(new Random().Next(100)).ContinueWith(delegate
				{
					if (Interlocked.Read(ref interlocker) == 0L)
					{
						Interlocked.Exchange(ref interlocker, 1L);
						WebApiCallback?.Invoke("TOKEN_EXPRIED", null);
						Interlocked.Exchange(ref interlocker, 0L);
					}
				});
			}
			responseModel2.success = responseModel.success;
			return responseModel2;
		}
		return new ResponseModel<object>
		{
			code = responseModel.code,
			content = responseModel.content,
			desc = responseModel.desc,
			success = responseModel.success
		};
	}

	public static async Task<bool> UploadAsync(string url, List<string> files, Dictionary<string, string> headers, bool extraHeader = false, bool addAuthorizationHeader = false)
	{
		if (headers == null)
		{
			headers = new Dictionary<string, string>();
		}
		if (extraHeader)
		{
			headers.Add("clientVersion", WebApiContext.CLIENT_VERSION);
			headers.Add("language", WebApiContext.LANGUAGE);
			headers.Add("windowsInfo", WebApiContext.WINDOWS_VERSION);
		}
		LogHelper.LogInstance.Debug("Request " + url + ", params: " + JsonHelper.SerializeObject2Json(headers));
		HttpClient httpClient = new HttpClient();
		try
		{
			MultipartFormDataContent formData = new MultipartFormDataContent("_-_-_-_-_");
			try
			{
				_ = 1;
				try
				{
					if (addAuthorizationHeader)
					{
						foreach (KeyValuePair<string, string> rEQUEST_AUTHOR_HEADER in WebApiContext.REQUEST_AUTHOR_HEADERS)
						{
							((HttpHeaders)httpClient.DefaultRequestHeaders).Add(rEQUEST_AUTHOR_HEADER.Key, rEQUEST_AUTHOR_HEADER.Value);
						}
					}
					foreach (KeyValuePair<string, string> header in headers)
					{
						string text = header.Value;
						if (string.IsNullOrEmpty(text))
						{
							text = string.Empty;
						}
						formData.Add((HttpContent)new StringContent(text, Encoding.UTF8), header.Key);
					}
					if (files != null && files.Count > 0)
					{
						files.ForEach(delegate(string n)
						{
							//IL_000e: Unknown result type (might be due to invalid IL or missing references)
							//IL_0014: Expected O, but got Unknown
							if (!string.IsNullOrEmpty(n))
							{
								HttpContent val = (HttpContent)new ByteArrayContent(File.ReadAllBytes(n));
								val.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
								formData.Add(val, "file", Path.GetFileName(n));
							}
						});
					}
					HttpResponseMessage val2 = await httpClient.PostAsync(url, (HttpContent)(object)formData);
					((object)val2)?.ToString();
					if (val2.StatusCode == HttpStatusCode.OK)
					{
						string text2 = await val2.Content.ReadAsStringAsync();
						LogHelper.LogInstance.Debug("Request " + url + ", Response whole result: " + text2);
						if (!string.IsNullOrEmpty(text2))
						{
							return JsonHelper.DeserializeJson2Object<ResponseModel<object>>(text2)?.code == "0000";
						}
					}
				}
				catch (Exception exception)
				{
					LogHelper.LogInstance.Error("upload file error:", exception);
				}
			}
			finally
			{
				if (formData != null)
				{
					((IDisposable)formData).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)httpClient)?.Dispose();
		}
		return false;
	}
}
