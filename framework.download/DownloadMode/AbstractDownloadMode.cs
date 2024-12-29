#define TRACE
using System.Diagnostics;
using System.IO;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadMode;

public abstract class AbstractDownloadMode : IDownload
{
	private string oldSpeed = string.Empty;

	private string newSpeed = string.Empty;

	protected int ReadWriteTimeout = 30000;

	public abstract DownloadStatus Start(DownloadTask task, CancellationTokenSource tokeSource);

	public abstract bool GetFileSizeFormServer(string url, ref long fileSize);

	protected virtual bool CheckBeforeDownload(DownloadTask task, ref DownloadStatus status, ref long offset)
	{
		if (File.Exists(Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.downloadFileName)))
		{
			status = DownloadStatus.SUCCESS;
			LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.MD5Check: File '{task.DownloadInfo.downloadFileName}' existsed");
			return false;
		}
		if (task.DownloadInfo.downloadFileSize <= 0)
		{
			long fileSize = 0L;
			if (!GetFileSizeFormServer(task.DownloadInfo.downloadUrl, ref fileSize) || fileSize < 0)
			{
				LogHelper.LogInstance.Info("lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.GetFileSizeFormServer: Get FileSize Form Server Failed");
				status = DownloadStatus.GETFILESIZEFAILED;
				return false;
			}
			task.DownloadInfo.downloadFileSize = fileSize;
		}
		if (HardDisk.GetHardDiskFreeSpace(task.DownloadInfo.saveLocalPath) <= task.DownloadInfo.downloadFileSize)
		{
			LogHelper.LogInstance.Info("lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.GetHardDiskFreeSpace: Have Not Enough Disk Space");
			status = DownloadStatus.UNENOUGHDISKSPACE;
			return false;
		}
		if (!Directory.Exists(task.DownloadInfo.saveLocalPath))
		{
			try
			{
				Directory.CreateDirectory(task.DownloadInfo.saveLocalPath);
			}
			catch
			{
				LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.GetHardDiskFreeSpace: Create Directory '{task.DownloadInfo.saveLocalPath}' Failed");
				status = DownloadStatus.CREATEDIRECTORYFAILED;
				return false;
			}
		}
		string text = Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.tempFileName);
		if (File.Exists(text))
		{
			FileInfo fileInfo = new FileInfo(text);
			if (fileInfo.Length >= task.DownloadInfo.downloadFileSize)
			{
				bool flag = true;
				if (!string.IsNullOrEmpty(task.DownloadInfo.downloadMD5))
				{
					flag = GlobalFun.MD5Check(text, task.DownloadInfo.downloadMD5);
				}
				if (flag)
				{
					GlobalFun.FileRename(text, Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.downloadFileName));
					LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.MD5Check: File '{task.DownloadInfo.downloadFileName}' MD5 Check Success");
					status = DownloadStatus.SUCCESS;
					return false;
				}
				LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.MD5Check: File '{task.DownloadInfo.downloadFileName}' MD5 Check Failed");
				GlobalFun.TryDeleteFile(text);
				status = DownloadStatus.MD5CHECKFAILED;
				return false;
			}
			offset = fileInfo.Length;
		}
		return true;
	}

	protected virtual DownloadStatus CheckAfterDownload(DownloadTask task, string path)
	{
		if (File.Exists(path))
		{
			if (new FileInfo(path).Length < task.DownloadInfo.downloadFileSize)
			{
				LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.CheckAfterDownload: File '{task.DownloadInfo.downloadFileName}' Download Pause");
				return DownloadStatus.PAUSE;
			}
			bool flag = true;
			if (!string.IsNullOrEmpty(task.DownloadInfo.downloadMD5))
			{
				flag = GlobalFun.MD5Check(path, task.DownloadInfo.downloadMD5);
			}
			if (flag)
			{
				LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.CheckAfterDownload: File '{task.DownloadInfo.downloadFileName}' MD5 Check Success");
				if (GlobalFun.FileRename(path, Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.downloadFileName)))
				{
					LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.CheckAfterDownload: File '{task.DownloadInfo.downloadFileName}' Download Success");
					return DownloadStatus.SUCCESS;
				}
				LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.CheckAfterDownload: File '{task.DownloadInfo.tempFileName}' Rename To '{task.DownloadInfo.downloadFileName}' Failed");
				return DownloadStatus.FILERENAMEFAILED;
			}
			LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.CheckAfterDownload: File '{task.DownloadInfo.downloadFileName}' MD5 Check Failed");
			GlobalFun.TryDeleteFile(path);
			return DownloadStatus.MD5CHECKFAILED;
		}
		LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadMode.AbstractDownloadMode.CheckAfterDownload: File '{task.DownloadInfo.downloadFileName}' Download Pause");
		return DownloadStatus.PAUSE;
	}

	protected void SpeedTimer(object state)
	{
		DownloadTask downloadTask = state as DownloadTask;
		newSpeed = GlobalFun.ConvertLong2String(downloadTask.totalSizeOfSec);
		downloadTask.totalSizeOfSec = 0L;
		if (!oldSpeed.Equals(newSpeed))
		{
			downloadTask.DownloadInfo.downloadSpeed = newSpeed;
			Trace.WriteLine(downloadTask.DownloadInfo.downloadSpeed);
			oldSpeed = newSpeed;
		}
	}
}
