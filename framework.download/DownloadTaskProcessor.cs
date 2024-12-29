using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download;

public class DownloadTaskProcessor
{
	private CancellationTokenSource tokenSource;

	private volatile bool isRuning;

	private object locker = new object();

	private DownloadTask task { get; set; }

	public DownloadTaskProcessor(DownloadTask task)
	{
		this.task = task;
	}

	public void Start()
	{
		DownloadStatus downloadStatus = DownloadStatus.DOWNLOADING;
		task.DownloadInfo.downloadStatus = downloadStatus;
		task.FireDownloadStatusChanged();
		if (!isRuning)
		{
			if (tokenSource == null)
			{
				tokenSource = new CancellationTokenSource();
			}
			Task.Factory.StartNew(DownloadThread, tokenSource.Token);
		}
	}

	public void Stop()
	{
		lock (locker)
		{
			while (isRuning)
			{
				if (tokenSource != null)
				{
					tokenSource.Cancel();
				}
				Thread.Sleep(10);
			}
		}
	}

	public void Delete()
	{
		Stop();
		TryRemoveDownloadFile();
	}

	public void Dispose()
	{
		if (tokenSource != null)
		{
			tokenSource.Dispose();
			tokenSource = null;
		}
	}

	private void DownloadThread()
	{
		try
		{
			SysSleepManagement.PreventSleep(includeDisplay: false);
			isRuning = true;
			LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadTaskProcessor: FileName: {task.DownloadInfo.downloadFileName}  Url: {task.DownloadInfo.downloadUrl} Start Download");
			if (tokenSource.IsCancellationRequested)
			{
				task.DownloadInfo.downloadStatus = DownloadStatus.PAUSE;
				task.DownloadInfo.downloadSpeed = "0.0B";
				return;
			}
			DownloadStatus downloadStatus = DownloadStatus.DOWNLOADING;
			int num = 0;
			do
			{
				downloadStatus = task.DownloadInfo.download.Start(task, tokenSource);
				if (downloadStatus == DownloadStatus.SUCCESS || downloadStatus == DownloadStatus.PAUSE)
				{
					break;
				}
				num++;
				Thread.Sleep(1000);
			}
			while (num < 1);
			task.DownloadInfo.downloadSpeed = "0.0B";
			task.DownloadInfo.downloadStatus = downloadStatus;
			task.EndTime = DateTime.Now;
		}
		catch (Exception exception)
		{
			task.DownloadInfo.downloadSpeed = "0.0B";
			task.DownloadInfo.downloadStatus = DownloadStatus.FAILED;
			LogHelper.LogInstance.Error($"lenovo.mbg.service.framework.download.DownloadTaskProcessor: FileName: {task.DownloadInfo.downloadFileName}  Url: {task.DownloadInfo.downloadUrl} Start Failed", exception);
		}
		finally
		{
			Dispose();
			LogHelper.LogInstance.Info($"lenovo.mbg.service.framework.download.DownloadTaskProcessor: FileName: {task.DownloadInfo.downloadFileName}  Url: {task.DownloadInfo.downloadUrl} Start Finished");
			isRuning = false;
			task.FireDownloadStatusChanged();
			SysSleepManagement.RestoreSleep();
		}
	}

	private void TryRemoveDownloadFile()
	{
		LogHelper.LogInstance.Error($"lenovo.mbg.service.framework.download.DownloadTaskProcessor.TryRemoveDownloadFile: RemoveFile FileName: {task.DownloadInfo.downloadFileName}  Url: {task.DownloadInfo.downloadUrl} Start");
		GlobalFun.TryDeleteFile(Path.Combine(task.DownloadInfo.saveLocalPath, task.DownloadInfo.tempFileName));
		LogHelper.LogInstance.Error($"lenovo.mbg.service.framework.download.DownloadTaskProcessor.TryRemoveDownloadFile: RemoveFile FileName: {task.DownloadInfo.downloadFileName}  Url: {task.DownloadInfo.downloadUrl} End");
	}
}
