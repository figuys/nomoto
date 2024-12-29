using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.framework.resources;

public class FileDownloadManagerV6 : IFileDownload
{
	protected ConcurrentDictionaryWrapper<string, DownloadWorker> DownloadingTasks;

	protected ConcurrentDictionaryWrapper<string, DownloadInfo> DownloadedTasks;

	private readonly int MaxDownloadingCount = 100;

	public int DownloadingCount => DownloadingTasks.GetValues().Count((DownloadWorker n) => n.Info.Status == DownloadStatus.DOWNLOADING);

	public event EventHandler<RemoteDownloadStatusEventArgs> OnRemoteDownloadStatusChanged;

	public FileDownloadManagerV6()
	{
		DownloadingTasks = new ConcurrentDictionaryWrapper<string, DownloadWorker>();
		DownloadedTasks = new ConcurrentDictionaryWrapper<string, DownloadInfo>();
	}

	public void Add(DownloadInfo resource, bool autoStart = true)
	{
		Add(new List<DownloadInfo> { resource }, autoStart);
	}

	public void Add(List<DownloadInfo> resources, bool autoStart = true)
	{
		lock (this)
		{
			if (resources == null || resources.Count <= 0)
			{
				return;
			}
			foreach (DownloadInfo resource in resources)
			{
				string fileUrl = resource.FileUrl;
				if (!DownloadingTasks.ContainsKey(fileUrl))
				{
					DownloadingTasks.AddOrUpdate(fileUrl, new DownloadWorker(resource, delegate(DownloadInfo info)
					{
						FireStatusChanged(info);
					}, delegate
					{
						ChangePathSuccessCallback();
					}));
					UpdateLocalDownloadingTask();
				}
				if (autoStart && DownloadingCount < MaxDownloadingCount)
				{
					ReStart(fileUrl);
				}
			}
		}
	}

	public void ReStart(string url)
	{
		DownloadWorker downloadWorker = DownloadingTasks.Get(url);
		downloadWorker.Info.ErrorMessage = null;
		downloadWorker.Info.Status = DownloadStatus.DOWNLOADING;
		FireStatusChanged(downloadWorker.Info);
		downloadWorker.Start();
	}

	public void Stop(string url)
	{
		DownloadWorker downloadWorker = DownloadingTasks.Get(url);
		downloadWorker.Info.Status = DownloadStatus.MANUAL_PAUSE;
		downloadWorker.Stop();
		FireStatusChanged(downloadWorker.Info);
	}

	public void Stop()
	{
		foreach (DownloadWorker value in DownloadingTasks.GetValues())
		{
			value.Info.Status = DownloadStatus.MANUAL_PAUSE;
			value.Stop();
			FireStatusChanged(value.Info);
		}
	}

	public void Delete(string url)
	{
		lock (this)
		{
			DownloadWorker downloadWorker = DownloadingTasks.Get(url);
			if (downloadWorker != null)
			{
				downloadWorker.Info.Status = DownloadStatus.DELETED;
				downloadWorker.Delete();
				DownloadingTasks.Remove(url);
				FireStatusChanged(downloadWorker.Info);
				UpdateLocalDownloadingTask();
				return;
			}
			DownloadInfo downloadInfo = DownloadedTasks.Get(url);
			if (downloadInfo != null)
			{
				downloadInfo.Status = DownloadStatus.DELETED;
				DownloadedTasks.Remove(url);
				FireStatusChanged(downloadInfo);
				string dirPath = Path.Combine(downloadInfo.LocalPath, Path.GetFileNameWithoutExtension(downloadInfo.FileName));
				GlobalFun.TryDeleteFile(Path.Combine(downloadInfo.LocalPath, downloadInfo.FileName));
				GlobalFun.DeleteDirectoryEx(dirPath);
				UpdateLocalDownloadedTask();
			}
		}
	}

	public DownloadInfo GetDownloadedResource(string url)
	{
		string filePath;
		return Rsd.Instance.GetDownloadedResource(url, out filePath);
	}

	public DownloadInfo GetDownloadingResource(string url)
	{
		return DownloadingTasks.Get(url)?.Info;
	}

	public Dictionary<DownloadInfoType, List<DownloadInfo>> Load()
	{
		try
		{
			Dictionary<DownloadInfoType, List<DownloadInfo>> dictionary = new Dictionary<DownloadInfoType, List<DownloadInfo>>();
			List<DownloadInfo> list = Rsd.Instance.InitDownloadedResources();
			List<DownloadInfo> value = Rsd.Instance.InitDownloadingResources();
			dictionary.Add(DownloadInfoType.DownloadingInfo, value);
			dictionary.Add(DownloadInfoType.DownloadedInfo, list);
			list?.ForEach(delegate(DownloadInfo n)
			{
				DownloadedTasks.AddOrUpdate(n.FileUrl, n);
			});
			return dictionary;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public void ChangePathSuccessCallback()
	{
		lock (this)
		{
			UpdateLocalDownloadingTask();
		}
	}

	private void FireStatusChanged(DownloadInfo e)
	{
		LogHelper.LogInstance.Debug($"{e.OriginalFileName} download status: {e.Status}");
		if (e.Status == DownloadStatus.SUCCESS || e.Status == DownloadStatus.ALREADYEXISTS || e.Status == DownloadStatus.UNZIPSUCCESS)
		{
			lock (this)
			{
				DownloadingTasks.Remove(e.FileUrl);
				DownloadInfo downloadInfo = DownloadedTasks.GetValues().FirstOrDefault((DownloadInfo n) => n.FileName.Equals(e.FileName));
				if (downloadInfo != null && !downloadInfo.FileUrl.Equals(e.FileUrl) && downloadInfo.LocalPath != e.LocalPath)
				{
					downloadInfo.DownloadUrl = e.DownloadUrl;
					downloadInfo.LocalPath = e.LocalPath;
					if (e.FileSize > 0)
					{
						downloadInfo.FileSize = e.FileSize;
					}
					downloadInfo.MD5 = e.MD5 ?? downloadInfo.MD5;
					downloadInfo.Status = e.Status;
					downloadInfo.ZipPwd = e.ZipPwd ?? downloadInfo.ZipPwd;
				}
				else
				{
					DownloadedTasks.AddOrUpdate(e.FileUrl, e);
				}
				UpdateLocalDownloadingTask();
				UpdateLocalDownloadedTask();
			}
			if (DownloadingCount < MaxDownloadingCount)
			{
				(from n in DownloadingTasks.GetValues()
					orderby n.Info.CreateDateTime
					select n).FirstOrDefault((DownloadWorker n) => n.Info.Status == DownloadStatus.WAITTING)?.Start();
			}
		}
		EventHandler<RemoteDownloadStatusEventArgs> eventHandler = this.OnRemoteDownloadStatusChanged;
		if (eventHandler != null)
		{
			Delegate[] invocationList = eventHandler.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<RemoteDownloadStatusEventArgs>)invocationList[i]).BeginInvoke(null, new RemoteDownloadStatusEventArgs(e.FileUrl, e.Status, e), null, null);
			}
		}
		if (DownloadingCount == 0)
		{
			SysSleepManagement.RestoreSleep();
		}
		else
		{
			SysSleepManagement.PreventSleep(includeDisplay: false);
		}
	}

	private void UpdateLocalDownloadingTask()
	{
		List<DownloadInfo> resources = (from n in DownloadingTasks.GetValues()
			select n.Info).ToList();
		Rsd.Instance.WriteDownloadingResources(resources);
	}

	private void UpdateLocalDownloadedTask()
	{
		List<DownloadInfo> values = DownloadedTasks.GetValues();
		Rsd.Instance.WriteDownloadedResources(values);
	}
}
