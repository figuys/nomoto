using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LmsaWindowsService.Common;

public class HttpUploadFile
{
	public static async Task<bool> Upload(string url, string filePath, Dictionary<string, string> aparams, int trycount, int reuploadwaitmilliseconds, Dictionary<string, string> authorHeader)
	{
		_ = 1;
		try
		{
			HttpContent val = (HttpContent)new StreamContent((Stream)new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
			val.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
			HttpClient client = new HttpClient();
			try
			{
				MultipartFormDataContent mulContent = new MultipartFormDataContent("----");
				try
				{
					foreach (KeyValuePair<string, string> item in authorHeader)
					{
						((HttpHeaders)client.DefaultRequestHeaders).Add(item.Key, item.Value);
					}
					if (aparams != null && aparams.Count > 0)
					{
						foreach (KeyValuePair<string, string> aparam in aparams)
						{
							mulContent.Add((HttpContent)new StringContent(aparam.Value ?? ""), aparam.Key);
						}
					}
					string[] array = Regex.Split(filePath, "\\\\|/");
					mulContent.Add(val, "file", array[array.Length - 1]);
					do
					{
						try
						{
							HttpResponseMessage response = await client.PostAsync(url, (HttpContent)(object)mulContent);
							await response.Content.ReadAsStringAsync();
							if (response.IsSuccessStatusCode)
							{
								return response.IsSuccessStatusCode;
							}
						}
						catch
						{
							Thread.Sleep(reuploadwaitmilliseconds);
						}
					}
					while (trycount-- > 0);
					return false;
				}
				finally
				{
					((IDisposable)mulContent)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)client)?.Dispose();
			}
		}
		catch (Exception)
		{
			return false;
		}
	}
}
