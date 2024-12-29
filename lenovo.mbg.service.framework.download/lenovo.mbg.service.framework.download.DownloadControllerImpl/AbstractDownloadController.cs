using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.download.DownloadSave;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download.DownloadControllerImpl;

public abstract class AbstractDownloadController
{
	public ConcurrentDictionary<AbstractDownloadInfo, DownloadTask> TaskDictionary;

	private object TaskDictionaryLock = new object();

	private object locker = new object();

	public Action<AbstractDownloadInfo> DownloadStatusChanged;

	public Action<AbstractDownloadInfo> ExistedDownloadTask;

	public Action<AbstractDownloadInfo> StopLowerControllerLevelTask;

	public int ControllerLevel { get; private set; }

	public int MaxDownloadCount { get; private set; }

	public ISaveDownloadInfoMode SaveMode { get; private set; }

	public int DownloadingCount => (from n in TaskDictionary.Values.AsParallel()
		where n.DownloadInfo.downloadStatus == DownloadStatus.DOWNLOADING
		select n).Count();

	public DownloadWorker worker => DownloadWorker.Instance;

	public AbstractDownloadController(ISaveDownloadInfoMode saveMode, int maxDownloadCount, int controllerLevel)
	{
		MaxDownloadCount = maxDownloadCount;
		ControllerLevel = controllerLevel;
		SaveMode = saveMode;
		TaskDictionary = new ConcurrentDictionary<AbstractDownloadInfo, DownloadTask>(new CompareDictionary());
		worker.OnDownloadStatusChanged += FireDownloadStatusChanged;
	}

	protected virtual void FireDownloadStatusChanged(object sender, DownloadEventArgs args)
	{
		if (!TaskDictionary.ContainsKey(args.downloadTask.DownloadInfo) || DownloadStatusChanged == null)
		{
			return;
		}
		SaveAbstractDownloadInfo();
		if (args.downloadTask.DownloadInfo.downloadStatus == DownloadStatus.SUCCESS)
		{
			lock (TaskDictionaryLock)
			{
				TaskDictionary.TryRemove(args.downloadTask.DownloadInfo, out var _);
			}
		}
		DownloadStatusChanged(args.downloadTask.DownloadInfo);
	}

	private void FireExistedDownloadTask(object sender, DownloadEventArgs args)
	{
		if (TaskDictionary.ContainsKey(args.downloadTask.DownloadInfo) && ExistedDownloadTask != null)
		{
			ExistedDownloadTask(args.downloadTask.DownloadInfo);
			SaveAbstractDownloadInfo();
		}
	}

	private void FireStopLowerControllerLevelTask(object sender, DownloadEventArgs args)
	{
		if (TaskDictionary.ContainsKey(args.downloadTask.DownloadInfo) && StopLowerControllerLevelTask != null)
		{
			StopLowerControllerLevelTask(args.downloadTask.DownloadInfo);
			SaveAbstractDownloadInfo();
		}
	}

	public virtual int Start(AbstractDownloadInfo downloadinfo)
	{
		DownloadTask downloadTask = new DownloadTask(downloadinfo, this);
		if (ExistedDownloadTask != null)
		{
			downloadTask.OnExistedDownloadTask += FireExistedDownloadTask;
		}
		if (StopLowerControllerLevelTask != null)
		{
			downloadTask.OnStopLowerControllerLevelTask += FireStopLowerControllerLevelTask;
		}
		if (!TaskDictionary.ContainsKey(downloadinfo))
		{
			lock (TaskDictionaryLock)
			{
				TaskDictionary.TryAdd(downloadinfo, downloadTask);
				SaveAbstractDownloadInfo();
			}
		}
		worker.Start(TaskDictionary[downloadinfo]);
		return 1;
	}

	public virtual int Stop(AbstractDownloadInfo downloadinfo)
	{
		if (TaskDictionary.ContainsKey(downloadinfo))
		{
			worker.Stop(TaskDictionary[downloadinfo]);
		}
		return 1;
	}

	public virtual int Stop()
	{
		lock (locker)
		{
			IEnumerable<KeyValuePair<AbstractDownloadInfo, DownloadTask>> enumerable = TaskDictionary.Where((KeyValuePair<AbstractDownloadInfo, DownloadTask> n) => n.Value.DownloadInfo.downloadStatus == DownloadStatus.DOWNLOADING);
			if (enumerable != null)
			{
				foreach (KeyValuePair<AbstractDownloadInfo, DownloadTask> item in enumerable)
				{
					worker.Stop(item.Value);
				}
			}
		}
		return 1;
	}

	public virtual int Delete(AbstractDownloadInfo downloadinfo)
	{
		Stop(downloadinfo);
		if (TaskDictionary.ContainsKey(downloadinfo))
		{
			worker.Delete(TaskDictionary[downloadinfo]);
			DownloadTask value = null;
			lock (TaskDictionaryLock)
			{
				TaskDictionary.TryRemove(downloadinfo, out value);
			}
			SaveAbstractDownloadInfo();
		}
		TaskDictionary.Values.Select((DownloadTask n) => n.DownloadInfo);
		return 1;
	}

	public virtual void SaveAbstractDownloadInfo()
	{
		if (SaveMode != null)
		{
			Task.Factory.StartNew(delegate
			{
				SaveMode.SaveAbstractDownloadInfo(SelectDownloadInfoFromTask());
			});
		}
	}

	public IEnumerable<AbstractDownloadInfo> SelectDownloadInfoFromTask()
	{
		return TaskDictionary.Values.Select((DownloadTask n) => n.DownloadInfo);
	}

	public virtual IEnumerable<AbstractDownloadInfo> GetAbstractDownloadInfoList()
	{
		try
		{
			if (SaveMode != null)
			{
				return SaveMode.GetAbstractDownloadInfoList();
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	private void StartNextTask()
	{
		IEnumerable<KeyValuePair<AbstractDownloadInfo, DownloadTask>> source = TaskDictionary.Where((KeyValuePair<AbstractDownloadInfo, DownloadTask> n) => n.Value.DownloadInfo.downloadStatus == DownloadStatus.WAITTING);
		if (source.Count() > 0)
		{
			DownloadTask value = (from n in source
				orderby n.Value.DownloadInfo.downloadLevel descending, n.Value.StartTime descending
				select n).First().Value;
			worker.Start(value);
		}
	}
}
