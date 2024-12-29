using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadMode;

public class FtpDownload : AbstractDownloadMode
{
	private const int MAX_BUFFER_LEN = 1024;

	public override DownloadStatus Start(DownloadTask task, CancellationTokenSource tokeSource)
	{
		if (tokeSource.IsCancellationRequested)
		{
			return DownloadStatus.PAUSE;
		}
		DownloadStatus status = DownloadStatus.DOWNLOADING;
		if (!task.DownloadInfo.downloadUrl.StartsWith("ftp", StringComparison.CurrentCultureIgnoreCase))
		{
			task.DownloadInfo.downloadUrl = "ftp://" + task.DownloadInfo.downloadUrl;
		}
		long offset = 0L;
		if (!CheckBeforeDownload(task, ref status, ref offset))
		{
			return status;
		}
		string path = Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.downloadFileName);
		FtpWebRequest ftpWebRequest = null;
		FileStream fileStream = null;
		Stream stream = null;
		Timer timer = null;
		try
		{
			byte[] buffer = new byte[1024];
			ftpWebRequest = OpenRequest(task.DownloadInfo.downloadUrl) as FtpWebRequest;
			ftpWebRequest.ReadWriteTimeout = ReadWriteTimeout;
			ftpWebRequest.ContentOffset = offset;
			using (FtpWebResponse ftpWebResponse = ftpWebRequest.GetResponse() as FtpWebResponse)
			{
				int num = 0;
				fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
				stream = ftpWebResponse.GetResponseStream();
				timer = new Timer(base.SpeedTimer, task, 0, 1000);
				while (!tokeSource.IsCancellationRequested)
				{
					try
					{
						int num2 = 0;
						while (++num2 < 10)
						{
							if (!stream.CanRead)
							{
								continue;
							}
							try
							{
								num = stream.Read(buffer, 0, 1024);
								offset += num;
								task.totalSizeOfSec += num;
								task.DownloadInfo.downloadedSize = offset;
								if (num <= 0 && fileStream.Length < task.DownloadInfo.downloadFileSize)
								{
									Thread.Sleep(1000);
									continue;
								}
							}
							catch (Exception)
							{
								continue;
							}
							break;
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
						continue;
					}
					catch (Exception exception)
					{
						LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.HttpDownload: Read Info From Stream Exception", exception);
						continue;
					}
				}
				stream.Close();
				ftpWebRequest.Abort();
				fileStream.Close();
				timer.Dispose();
				LogHelper.LogInstance.Info("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: Download Finished, But Not Check MD5");
			}
			return CheckAfterDownload(task, path);
		}
		catch (WebException ex2)
		{
			if (ex2.Status == WebExceptionStatus.ProtocolError)
			{
				LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: WebException Exception - WebExceptionStatus.ProtocolError", ex2);
				return ((ex2.Response as FtpWebResponse).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) ? DownloadStatus.DOWNLOADFILENOTFOUND : DownloadStatus.UNDEFINEERROR;
			}
			if (ex2.Status == WebExceptionStatus.ConnectFailure || ex2.Status == WebExceptionStatus.ConnectionClosed || ex2.Status == WebExceptionStatus.Timeout)
			{
				LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: WebException Exception - WebExceptionStatus.ConnectFailure", ex2);
				return DownloadStatus.NETWORKCONNECTIONERROR;
			}
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: WebException Exception - UNDEFINEERROR", ex2);
			return DownloadStatus.UNDEFINEERROR;
		}
		catch (SocketException exception2)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: SocketException Exception - UNDEFINEERROR", exception2);
			return DownloadStatus.NETWORKCONNECTIONERROR;
		}
		catch (Exception exception3)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: Exception", exception3);
			return DownloadStatus.UNDEFINEERROR;
		}
		finally
		{
			try
			{
				stream.Close();
				ftpWebRequest.Abort();
				fileStream.Close();
				timer.Dispose();
			}
			catch
			{
			}
			LogHelper.LogInstance.Info("lenovo.mbg.service.framework.download.DownloadMode.FtpDownload: Download Finished");
		}
	}

	private WebRequest OpenRequest(string requestUri, string method, int timeout)
	{
		try
		{
			FtpWebRequest obj = WebRequest.Create(requestUri) as FtpWebRequest;
			obj.Method = method;
			obj.UseBinary = true;
			obj.UsePassive = true;
			obj.Timeout = timeout;
			obj.ReadWriteTimeout = timeout;
			obj.Proxy = WebRequest.DefaultWebProxy;
			return obj;
		}
		catch
		{
			return null;
		}
	}

	private WebRequest OpenRequest(string requestUri)
	{
		return OpenRequest(requestUri, "RETR", 30000);
	}

	public override bool GetFileSizeFormServer(string url, ref long fileSize)
	{
		FtpWebRequest ftpWebRequest = null;
		FtpWebResponse ftpWebResponse = null;
		try
		{
			ftpWebRequest = OpenRequest(url, "SIZE", 30000) as FtpWebRequest;
			ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
			fileSize = ftpWebResponse.ContentLength;
			return true;
		}
		catch
		{
			return false;
		}
		finally
		{
			ftpWebResponse.Close();
			ftpWebRequest.Abort();
			ftpWebResponse = null;
			ftpWebRequest = null;
		}
	}
}
