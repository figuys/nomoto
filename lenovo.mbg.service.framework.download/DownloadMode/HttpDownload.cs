using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadMode;

public class HttpDownload : AbstractDownloadMode
{
	private const int MAX_BUFFER_LEN = 1024;

	public override DownloadStatus Start(DownloadTask task, CancellationTokenSource tokeSource)
	{
		if (tokeSource.IsCancellationRequested)
		{
			return DownloadStatus.PAUSE;
		}
		DownloadStatus status = DownloadStatus.DOWNLOADING;
		if (!task.DownloadInfo.downloadUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) && !task.DownloadInfo.downloadUrl.StartsWith("https", StringComparison.CurrentCultureIgnoreCase))
		{
			task.DownloadInfo.downloadUrl = "http://" + task.DownloadInfo.downloadUrl;
		}
		long offset = 0L;
		if (!CheckBeforeDownload(task, ref status, ref offset))
		{
			return status;
		}
		string path = Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.tempFileName);
		HttpWebRequest httpWebRequest = null;
		FileStream fileStream = null;
		Stream stream = null;
		Timer timer = null;
		try
		{
			if (tokeSource.IsCancellationRequested)
			{
				return DownloadStatus.PAUSE;
			}
			byte[] buffer = new byte[1024];
			httpWebRequest = OpenRequest(task.DownloadInfo.downloadUrl);
			httpWebRequest.AddRange((int)offset);
			using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
			{
				int num = 0;
				fileStream = File.Open(path, FileMode.Append, FileAccess.Write);
				stream = httpWebResponse.GetResponseStream();
				timer = new Timer(base.SpeedTimer, task, 0, 1000);
				while (!tokeSource.IsCancellationRequested)
				{
					int num2 = 0;
					while (++num2 < 30)
					{
						if (stream.CanRead)
						{
							num = stream.Read(buffer, 0, 1024);
							offset += num;
							task.totalSizeOfSec += num;
							task.DownloadInfo.downloadedSize = offset;
							if (num > 0 || fileStream.Length >= task.DownloadInfo.downloadFileSize)
							{
								break;
							}
							Thread.Sleep(2000);
						}
					}
					if (fileStream.Length >= task.DownloadInfo.downloadFileSize)
					{
						break;
					}
					if (num <= 0)
					{
						return DownloadStatus.PAUSE;
					}
					fileStream.Write(buffer, 0, num);
				}
				stream.Close();
				httpWebRequest.Abort();
				fileStream.Close();
				timer.Dispose();
				LogHelper.LogInstance.Info("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: Download Finished, But Not Check MD5");
			}
			return CheckAfterDownload(task, path);
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: WebException Exception - WebExceptionStatus.ProtocolError", ex);
				return ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound) ? DownloadStatus.DOWNLOADFILENOTFOUND : DownloadStatus.UNDEFINEERROR;
			}
			if (ex.Status == WebExceptionStatus.ConnectFailure || ex.Status == WebExceptionStatus.ConnectionClosed || ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.NameResolutionFailure)
			{
				LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: WebException Exception - WebExceptionStatus.ConnectFailure", ex);
				return DownloadStatus.NETWORKCONNECTIONERROR;
			}
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: WebException Exception - UNDEFINEERROR", ex);
			return DownloadStatus.UNDEFINEERROR;
		}
		catch (SocketException exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: SocketException Exception - UNDEFINEERROR", exception);
			return DownloadStatus.NETWORKCONNECTIONERROR;
		}
		catch (Exception exception2)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: Exception", exception2);
			return DownloadStatus.UNDEFINEERROR;
		}
		finally
		{
			try
			{
				stream?.Close();
				httpWebRequest?.Abort();
				fileStream?.Close();
				timer?.Dispose();
			}
			catch
			{
			}
			LogHelper.LogInstance.Info("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: Download Finished");
		}
	}

	public override bool GetFileSizeFormServer(string url, ref long fileSize)
	{
		long num = 0L;
		bool result = false;
		HttpWebRequest httpWebRequest = null;
		HttpWebResponse httpWebResponse = null;
		try
		{
			httpWebRequest = OpenRequest(url, "HEAD", 30000);
			if (httpWebRequest != null)
			{
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					num = httpWebResponse.ContentLength;
					result = true;
				}
				else
				{
					num = 0L;
					result = false;
				}
			}
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				try
				{
					int result2 = 0;
					httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
					httpWebRequest.Method = "GET";
					httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					if (int.TryParse(httpWebResponse.Headers["Content-Length"], out result2))
					{
						num = result2;
						result = true;
					}
				}
				catch
				{
				}
			}
			else
			{
				if (ex.Status == WebExceptionStatus.Timeout)
				{
					httpWebRequest.Abort();
				}
				num = 0L;
				result = false;
			}
		}
		catch
		{
			num = 0L;
			result = false;
		}
		finally
		{
			if (httpWebResponse != null)
			{
				httpWebResponse.Close();
				httpWebResponse = null;
			}
			if (httpWebRequest != null)
			{
				httpWebRequest.Abort();
				httpWebRequest = null;
			}
		}
		fileSize = num;
		return result;
	}

	private HttpWebRequest OpenRequest(string serverFileUrl)
	{
		return OpenRequest(serverFileUrl, "GET", 30000);
	}

	private HttpWebRequest OpenRequest(string serverFileUrl, string method, int timeout)
	{
		try
		{
			HttpWebRequest obj = WebRequest.Create(serverFileUrl) as HttpWebRequest;
			obj.Method = method;
			obj.UserAgent = GlobalVar.UserAgent;
			obj.Timeout = timeout;
			obj.KeepAlive = false;
			return obj;
		}
		catch
		{
			return null;
		}
	}
}
