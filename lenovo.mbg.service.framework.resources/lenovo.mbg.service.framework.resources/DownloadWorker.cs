using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.resources;

public class DownloadWorker
{
	protected HttpWebRequestState ReqState;

	protected DownloadStatus status;

	protected int RetryCount = 3;

	protected ManualResetEvent WorkLockEvent;

	protected Action<DownloadInfo> CallbackAction;

	protected Action ChangePathSuccessAction;

	protected bool IsDelete;

	public bool Running { get; private set; }

	public DownloadInfo Info { get; }

	protected string TempPath { get; private set; }

	protected string uri { get; set; }

	public DownloadWorker(DownloadInfo info, Action<DownloadInfo> action, Action changePathSuccessAction)
	{
		Info = info;
		CallbackAction = action;
		ChangePathSuccessAction = changePathSuccessAction;
		WorkLockEvent = new ManualResetEvent(initialState: false);
		uri = (info.DownloadUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? info.DownloadUrl : ("http://" + info.DownloadUrl));
		ValidateDownloadUrl();
		if (string.IsNullOrEmpty(info.LocalPath))
		{
			info.LocalPath = Configurations.FileSaveLocalPath[info.FileType];
		}
		TempPath = Path.Combine(info.LocalPath, info.FileName + ".tmp");
	}

	public async void Start()
	{
		Info.ErrorMessage = null;
		RetryCount = 3;
		WorkLockEvent.Set();
		if (Running)
		{
			return;
		}
		Running = true;
		await Task.Run(delegate
		{
			if (!ExistsCheck())
			{
				MoveFile();
				HttpDownloadAsync();
			}
			else
			{
				Running = false;
			}
		});
	}

	private void MoveFile()
	{
		string text = Configurations.FileSaveLocalPath[Info.FileType];
		if (!(Info.LocalPath != text))
		{
			return;
		}
		LogHelper.LogInstance.Info("Change save path to: " + text);
		bool flag = false;
		if (ReqState != null && ReqState.FStream != null)
		{
			ReqState.FStream.Flush();
			ReqState.FStream.Dispose();
			ReqState.FStream = null;
			flag = true;
		}
		string text2 = Path.Combine(text, Info.FileName + ".tmp");
		if (File.Exists(TempPath))
		{
			try
			{
				GlobalFun.TryDeleteFile(text2);
				File.Move(TempPath, text2);
			}
			catch (Exception)
			{
			}
		}
		TempPath = text2;
		Info.LocalPath = text;
		Task.Run(delegate
		{
			ChangePathSuccessAction?.Invoke();
		});
		if (flag)
		{
			ReqState.DiskInfo = new DriveInfo(Path.GetPathRoot(Info.LocalPath));
			ReqState.FStream = new FileStream(TempPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, ReqState.MAX_BUFFER_SIZE, useAsync: true);
		}
	}

	public void Stop()
	{
		WorkLockEvent.Reset();
	}

	public async void Delete()
	{
		IsDelete = true;
		if (!Running)
		{
			await Task.Run(delegate
			{
				GlobalFun.TryDeleteFile(TempPath);
				GlobalFun.TryDeleteFile(Path.Combine(Info.LocalPath, Info.FileName));
			});
		}
	}

	private bool CheckRunningStatus()
	{
		if (IsDelete || !WorkLockEvent.WaitOne(2000))
		{
			status = (IsDelete ? DownloadStatus.DELETED : DownloadStatus.MANUAL_PAUSE);
			IsDelete = false;
			Free();
			return false;
		}
		if (ReqState.DiskInfo.AvailableFreeSpace < 20480000)
		{
			RetryCount = 0;
			status = DownloadStatus.UNENOUGHDISKSPACE;
			Free();
			return false;
		}
		MoveFile();
		return true;
	}

	private bool ExistsCheck()
	{
		bool flag = GlobalFun.Exists(Path.Combine(Info.LocalPath, Info.FileName));
		if (Info.UnZip && Info.FileType != "TOOL")
		{
			if (flag)
			{
				flag = UnZip(unzip: false);
				if (!flag)
				{
					FireStatusChangedCallback(DownloadStatus.DOWNLOADING);
				}
			}
			else
			{
				flag = GlobalFun.Exists(Path.Combine(Info.LocalPath, Path.GetFileNameWithoutExtension(Info.FileName)));
				if (flag)
				{
					FireStatusChangedCallback(DownloadStatus.UNZIPSUCCESS);
				}
			}
		}
		else if (flag)
		{
			FireStatusChangedCallback(DownloadStatus.SUCCESS);
		}
		return flag;
	}

	private bool UnZip(bool unzip)
	{
		FireStatusChangedCallback(DownloadStatus.UNZIPPING);
		bool result = true;
		switch (Rsd.Instance.Unzip(Info, unzip))
		{
		case 0:
			status = DownloadStatus.UNZIPSUCCESS;
			break;
		case 1:
			status = DownloadStatus.UNZIPNOSPACE;
			break;
		case 4:
			status = DownloadStatus.SUCCESS;
			break;
		default:
			status = DownloadStatus.UNZIPFAILED;
			result = false;
			GlobalFun.TryDeleteFile(Path.Combine(Info.LocalPath, Info.FileName));
			break;
		}
		FireStatusChangedCallback(status);
		return result;
	}

	private void ValidateDownloadUrl()
	{
		if (string.IsNullOrEmpty(uri))
		{
			return;
		}
		bool flag = true;
		Match match = Regex.Match(uri, "(?<key>expires=)(?<value>\\d+)", RegexOptions.IgnoreCase);
		if (match.Success)
		{
			int num = int.Parse(match.Groups["value"].Value);
			match = Regex.Match(uri, "(?<key>X-Amz-Date=)(?<value>[^&]+)", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				DateTime dateTime = DateTime.Parse(Regex.Replace(match.Groups["value"].Value, "(.{4})()(.{2})()(.{2})(.)(.{2})()(.{2})()(.+)", "$1-$3-$5$6$7:$9:$11")).ToUniversalTime().AddSeconds(num);
				LogHelper.LogInstance.Debug($"download link expries utc time: {dateTime.ToString()}, current utc time: {DateTime.UtcNow.ToString()}");
				flag = DateTime.UtcNow < dateTime;
			}
			else
			{
				flag = DateTime.UtcNow < new DateTime(1970, 1, 1).AddSeconds(num);
			}
		}
		if (!flag)
		{
			uri = Info.DownloadUrl;
		}
	}

	private bool BeforeCheck()
	{
		if (Info.FileSize <= 0)
		{
			int num = 3;
			do
			{
				if (GlobalFun.GetFileSize(uri, out var filesize))
				{
					Info.FileSize = filesize;
					break;
				}
				Thread.Sleep(1000);
			}
			while (--num > 0);
			if (num == 0)
			{
				RetryCount = 0;
				LogHelper.LogInstance.Info(uri + ", get file size failed");
				status = DownloadStatus.GETFILESIZEFAILED;
				return false;
			}
		}
		if (ReqState.DiskInfo.AvailableFreeSpace <= Info.FileSize)
		{
			RetryCount = 0;
			LogHelper.LogInstance.Info($"Have not enough disk space: {Path.GetPathRoot(Info.LocalPath)}");
			status = DownloadStatus.UNENOUGHDISKSPACE;
			return false;
		}
		return true;
	}

	private void AfterCheck()
	{
		if (Info.LocalFileSize >= Info.FileSize)
		{
			Info.Progress = 100.0;
			if (string.IsNullOrEmpty(Info.MD5) || GlobalFun.MD5Check(TempPath, Info.MD5))
			{
				GlobalFun.FileRename(TempPath, Path.Combine(Info.LocalPath, Info.FileName));
				status = DownloadStatus.SUCCESS;
				return;
			}
			GlobalFun.TryDeleteFile(TempPath);
			Info.LocalFileSize = 0L;
			LogHelper.LogInstance.Info("Download file " + Info.DownloadUrl + ", server md5: " + Info.MD5 + " check failed");
			status = DownloadStatus.MD5CHECKFAILED;
		}
	}

	private void HttpDownloadAsync()
	{
		try
		{
			status = DownloadStatus.DOWNLOADING;
			GC.Collect();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.DefaultConnectionLimit = 50;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(uri));
			httpWebRequest.Method = "GET";
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36";
			httpWebRequest.Headers.Add("Request-Tag: lmsa");
			httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			httpWebRequest.KeepAlive = false;
			httpWebRequest.ReadWriteTimeout = 5000;
			ReqState = new HttpWebRequestState();
			ReqState.FStream = new FileStream(TempPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, ReqState.MAX_BUFFER_SIZE, useAsync: true);
			ReqState.DiskInfo = new DriveInfo(Path.GetPathRoot(Info.LocalPath));
			ReqState.Request = httpWebRequest;
			ReqState.StartTime = DateTime.Now;
			if (ReqState.FStream.Length > 0)
			{
				httpWebRequest.AddRange(ReqState.FStream.Length);
			}
			if (CheckRunningStatus())
			{
				httpWebRequest.BeginGetResponse(RespCallback, ReqState);
			}
		}
		catch (Exception exception)
		{
			ProcessException(exception);
		}
	}

	private void RespCallback(IAsyncResult asyncResult)
	{
		try
		{
			HttpWebRequestState httpWebRequestState = (HttpWebRequestState)asyncResult.AsyncState;
			HttpWebResponse response = (HttpWebResponse)httpWebRequestState.Request.EndGetResponse(asyncResult);
			httpWebRequestState.Response = response;
			ReqState.ReadAllLength = ReqState.FStream.Length;
			Info.LocalFileSize = ReqState.ReadAllLength;
			if (BeforeCheck())
			{
				Stream responseStream = httpWebRequestState.Response.GetResponseStream();
				responseStream.ReadTimeout = 5000;
				httpWebRequestState.ResponseStream = responseStream;
				responseStream.BeginRead(httpWebRequestState.BufferRead[httpWebRequestState.BufferReadIndex], 0, httpWebRequestState.MAX_BUFFER_SIZE, ReadCallback, httpWebRequestState);
			}
			else
			{
				Free();
			}
		}
		catch (Exception exception)
		{
			ProcessException(exception);
		}
	}

	private void ReadCallback(IAsyncResult asyncResult)
	{
		bool throwException = true;
		try
		{
			HttpWebRequestState httpWebRequestState = (HttpWebRequestState)asyncResult.AsyncState;
			Stream responseStream = httpWebRequestState.ResponseStream;
			int num = responseStream.EndRead(asyncResult);
			if (ReqState != null && ReqState.WaitHandle != null)
			{
				ReqState.WaitHandle.Unregister(asyncResult.AsyncWaitHandle);
			}
			if (num > 0)
			{
				httpWebRequestState.SecTotalByte += num;
				TimeSpan timeSpan = DateTime.Now - httpWebRequestState.StartTime;
				if (timeSpan.TotalMilliseconds > 1000.0)
				{
					Info.Speed = ((Info.Status != DownloadStatus.DOWNLOADING) ? "0 KB/S" : (GlobalFun.ConvertLong2String((long)((double)httpWebRequestState.SecTotalByte / timeSpan.TotalMilliseconds) * 1000) + "/S"));
					long num2 = (Info.FileSize - httpWebRequestState.ReadAllLength) / httpWebRequestState.SecTotalByte;
					TimeSpan timeSpan2 = new TimeSpan(0, 0, (int)num2);
					if (timeSpan2.Days > 0)
					{
						Info.NeedTakesTime = "--mins lefts";
					}
					else
					{
						Info.NeedTakesTime = timeSpan2.ToString("hh\\:mm\\:ss");
					}
					httpWebRequestState.SecTotalByte = 0L;
					httpWebRequestState.StartTime = DateTime.Now;
				}
				httpWebRequestState.ReadLength = num;
				httpWebRequestState.FStream.BeginWrite(httpWebRequestState.BufferRead[httpWebRequestState.BufferReadIndex], 0, num, WriteCallback, httpWebRequestState);
				if (!httpWebRequestState.WriteDoneEvent.WaitOne(5000))
				{
					throw new Exception("byte stream writing to file timeout, timeout period 5000 milliseconds");
				}
				httpWebRequestState.BufferReadIndex ^= 1;
				if (!CheckRunningStatus())
				{
					return;
				}
				if (!responseStream.CanRead)
				{
					responseStream.Close();
					httpWebRequestState.ResponseStream = httpWebRequestState.Response.GetResponseStream();
					responseStream = httpWebRequestState.ResponseStream;
					responseStream.ReadTimeout = 5000;
				}
				Array.Clear(httpWebRequestState.BufferRead[httpWebRequestState.BufferReadIndex], 0, httpWebRequestState.MAX_BUFFER_SIZE);
				IAsyncResult asyncResult2 = responseStream.BeginRead(httpWebRequestState.BufferRead[httpWebRequestState.BufferReadIndex], 0, httpWebRequestState.MAX_BUFFER_SIZE, ReadCallback, httpWebRequestState);
				if (ReqState == null)
				{
					return;
				}
				ReqState.WaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult2.AsyncWaitHandle, delegate(object s, bool t)
				{
					if (t)
					{
						throwException = false;
						ReqState.ResponseStream?.Close();
					}
				}, responseStream, 5000, executeOnlyOnce: true);
			}
			else
			{
				Free();
			}
		}
		catch (Exception exception)
		{
			ProcessException(exception, throwException);
		}
	}

	private void WriteCallback(IAsyncResult asyncResult)
	{
		HttpWebRequestState httpWebRequestState = (HttpWebRequestState)asyncResult.AsyncState;
		if (httpWebRequestState != null)
		{
			if (httpWebRequestState.FStream != null)
			{
				httpWebRequestState.FStream.EndWrite(asyncResult);
				Info.LocalFileSize += httpWebRequestState.ReadLength;
				httpWebRequestState.ReadAllLength += httpWebRequestState.ReadLength;
			}
			if (!httpWebRequestState.WriteDoneEvent.SafeWaitHandle.IsClosed && !httpWebRequestState.WriteDoneEvent.SafeWaitHandle.IsInvalid)
			{
				httpWebRequestState.WriteDoneEvent.Set();
			}
		}
	}

	private void ProcessException(Exception exception, bool throwException = true)
	{
		if (throwException || RetryCount <= 1)
		{
			LogHelper.LogInstance.Error("File download occur an exception: " + Info.DownloadUrl);
			if (exception is WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					LogHelper.LogInstance.Error("web exception - ProtocolError", exception);
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
					{
						status = DownloadStatus.FAILED;
						ReqState.ResponseStream?.Close();
						ReqState.Response?.Close();
						ReqState.Request?.Abort();
						ReqState.FStream.Flush();
						Info.LocalFileSize = ReqState.FStream.Length;
						ReqState.FStream.Dispose();
						ReqState.FStream = null;
						ReqState = null;
						LogHelper.LogInstance.Info($"server file size: {Info.FileSize}, local tmp file size: {Info.LocalFileSize}");
						Info.FileSize = 0L;
						GlobalFun.TryDeleteFile(TempPath);
					}
					else
					{
						status = ((httpWebResponse.StatusCode == HttpStatusCode.NotFound) ? DownloadStatus.DOWNLOADFILENOTFOUND : DownloadStatus.UNDEFINEERROR);
					}
				}
				else if (ex.Status == WebExceptionStatus.ConnectFailure || ex.Status == WebExceptionStatus.ConnectionClosed || ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.NameResolutionFailure)
				{
					LogHelper.LogInstance.Error("web exception - ConnectFailure", exception);
					status = DownloadStatus.NETWORKCONNECTIONERROR;
				}
				else
				{
					LogHelper.LogInstance.Error("web exception - Unknown", exception);
					status = DownloadStatus.UNDEFINEERROR;
				}
			}
			else if (exception is SocketException exception2)
			{
				LogHelper.LogInstance.Error("socket exception", exception2);
				status = DownloadStatus.NETWORKCONNECTIONERROR;
			}
			else
			{
				LogHelper.LogInstance.Error("other exception", exception);
				status = DownloadStatus.UNDEFINEERROR;
			}
		}
		Free();
	}

	private void Free()
	{
		Info.Speed = "0 KB/S";
		if (ReqState != null)
		{
			ReqState.ResponseStream?.Close();
			ReqState.Response?.Close();
			ReqState.Request?.Abort();
			ReqState.FStream.Flush();
			Info.LocalFileSize = ReqState.FStream.Length;
			ReqState.FStream.Dispose();
			ReqState.FStream = null;
			ReqState = null;
		}
		if (status == DownloadStatus.MANUAL_PAUSE)
		{
			FireFinishedCallback();
		}
		else if (status == DownloadStatus.DELETED)
		{
			GlobalFun.TryDeleteFile(TempPath);
			GlobalFun.TryDeleteFile(Path.Combine(Info.LocalPath, Info.FileName));
			FireFinishedCallback();
		}
		else if (status == DownloadStatus.DOWNLOADING)
		{
			if (Info.LocalFileSize >= Info.FileSize)
			{
				AfterCheck();
				FireFinishedCallback();
			}
			else
			{
				HttpDownloadAsync();
			}
		}
		else if (--RetryCount > 0)
		{
			HttpDownloadAsync();
		}
		else
		{
			FireFinishedCallback();
		}
	}

	private void SetErrorMessage()
	{
		switch (status)
		{
		case DownloadStatus.UNZIPFAILED:
			Info.ErrorMessage = "K0329";
			break;
		case DownloadStatus.UNZIPNOSPACE:
			Info.ErrorMessage = "K1347";
			break;
		case DownloadStatus.MD5CHECKFAILED:
			Info.ErrorMessage = "K0323";
			break;
		case DownloadStatus.FAILED:
		case DownloadStatus.GETFILESIZEFAILED:
		case DownloadStatus.CREATEDIRECTORYFAILED:
		case DownloadStatus.UNDEFINEERROR:
		case DownloadStatus.NETWORKCONNECTIONERROR:
			Info.ErrorMessage = "K0324";
			break;
		case DownloadStatus.UNENOUGHDISKSPACE:
			Info.ErrorMessage = "K0325";
			break;
		case DownloadStatus.DOWNLOADFILENOTFOUND:
			Info.ErrorMessage = "K0326";
			break;
		case DownloadStatus.FILERENAMEFAILED:
			Info.ErrorMessage = "K0330";
			break;
		default:
			Info.ErrorMessage = null;
			break;
		}
	}

	private void FireFinishedCallback()
	{
		if (status == DownloadStatus.MANUAL_PAUSE && Info.Status == DownloadStatus.DOWNLOADING)
		{
			Running = false;
			Start();
		}
		else if (status == DownloadStatus.SUCCESS && Info.UnZip)
		{
			Info.ZipPwd = Security.Instance.EncryptAseString(Security.Instance.GetRandomString(6, useNum: true, useLow: true, useUpp: true, useSpe: false, ""));
			UnZip(unzip: true);
			Running = false;
		}
		else
		{
			Running = false;
			FireStatusChangedCallback(status);
		}
	}

	private void FireStatusChangedCallback(DownloadStatus status)
	{
		Info.Status = status;
		SetErrorMessage();
		CallbackAction?.BeginInvoke(Info, null, null);
		if (status == DownloadStatus.UNZIPSUCCESS && Info.FileType == "ROM")
		{
			SaveFilesInfoAsync();
		}
	}

	private async void SaveFilesInfoAsync()
	{
		await Task.Run(delegate
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Info.FileName);
			string directory = Path.Combine(Info.LocalPath, fileNameWithoutExtension);
			JArray jArray = new JArray();
			List<string> allFiles = GlobalFun.GetAllFiles(directory);
			if (allFiles != null && allFiles.Count > 0)
			{
				foreach (string item in allFiles)
				{
					FileInfo fileInfo = new FileInfo(item);
					jArray.Add(new JObject
					{
						{ "Path", fileInfo.FullName },
						{ "Name", fileInfo.Name },
						{
							"LastModifiedTime",
							GlobalFun.ToUtcTimeStamp(fileInfo.LastWriteTime)
						},
						{ "Size", fileInfo.Length }
					});
				}
			}
			if (jArray.HasValues)
			{
				string content = JsonHelper.SerializeObject2FormatJson(jArray);
				FileHelper.WriteFileWithAesEncrypt(Path.Combine(Configurations.DownloadInfoSavePath, fileNameWithoutExtension + ".check.json.dpapi"), content, FileAttributes.Hidden);
			}
		});
	}
}
