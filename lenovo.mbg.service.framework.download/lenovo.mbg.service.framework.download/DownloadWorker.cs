using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.framework.download;

public sealed class DownloadWorker
{
	private static object locker = new object();

	private static DownloadWorker m_woker;

	private ConcurrentDictionary<AbstractDownloadInfo, DownloadTask> TaskDictionary;

	private object lo = new object();

	public static DownloadWorker Instance
	{
		get
		{
			if (m_woker == null)
			{
				lock (locker)
				{
					if (m_woker == null)
					{
						m_woker = new DownloadWorker();
					}
				}
			}
			return m_woker;
		}
	}

	public int DownloadMaxCount { get; private set; }

	public int DownloadingCount => (from n in TaskDictionary.Values.AsParallel()
		where n.DownloadInfo.downloadStatus == DownloadStatus.DOWNLOADING
		select n).Count();

	public event EventHandler<DownloadEventArgs> OnDownloadStatusChanged;

	private DownloadWorker()
	{
		try
		{
			LoadXml();
		}
		catch
		{
			DownloadMaxCount = 1;
		}
		TaskDictionary = new ConcurrentDictionary<AbstractDownloadInfo, DownloadTask>(new CompareDictionary());
	}

	public void LoadXml()
	{
		using XmlReader xmlReader = XmlReader.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download-config.xml"));
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.Equals("MaxDownloadCount"))
			{
				DownloadMaxCount = int.Parse(xmlReader.Value);
				break;
			}
		}
	}

	public void InitializeMaxDownloadCount(int count)
	{
		DownloadMaxCount = count;
	}

	private DownloadTask FindLowerLevelTask()
	{
		lock (lo)
		{
			try
			{
				IEnumerable<DownloadTask> downloadingTasks = TaskDictionary.Values.Where((DownloadTask n) => n.DownloadInfo.downloadStatus == DownloadStatus.DOWNLOADING);
				IEnumerable<DownloadTask> lowerControllerTasks = downloadingTasks.Where((DownloadTask n) => n.Controller.ControllerLevel == downloadingTasks.AsParallel().Min((DownloadTask m) => m.Controller.ControllerLevel));
				IEnumerable<DownloadTask> lowerDownloadLevelTasks = lowerControllerTasks.Where((DownloadTask n) => n.DownloadInfo.downloadLevel == lowerControllerTasks.AsParallel().Min((DownloadTask m) => m.DownloadInfo.downloadLevel));
				return lowerDownloadLevelTasks.FirstOrDefault((DownloadTask n) => n.DownloadInfo.downloadedSize == lowerDownloadLevelTasks.Min((DownloadTask m) => m.DownloadInfo.downloadedSize));
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public void Start(DownloadTask task)
	{
		lock (lo)
		{
			if (!TaskDictionary.ContainsKey(task.DownloadInfo))
			{
				task.OnDownloadStatusChanged += FireDownloadStatusChanged;
				TaskDictionary.TryAdd(task.DownloadInfo, task);
				if (DownloadMaxCount > GetDownloadingCount())
				{
					task.downloadTaskProcessor.Start();
					return;
				}
				DownloadTask downloadTask = FindLowerLevelTask();
				if (downloadTask != null && task.Controller.ControllerLevel > downloadTask.Controller.ControllerLevel)
				{
					downloadTask.FireStopLowerControllerLevelTask();
					downloadTask.downloadTaskProcessor.Stop();
				}
				task.downloadTaskProcessor.Start();
			}
			else if (task.Controller.ControllerLevel == TaskDictionary[task.DownloadInfo].Controller.ControllerLevel && TaskDictionary[task.DownloadInfo].DownloadInfo.downloadStatus == DownloadStatus.DOWNLOADING)
			{
				task.FireExistedDownloadTask(new DownloadEventArgs(TaskDictionary[task.DownloadInfo]));
				TaskDictionary[task.DownloadInfo].downloadTaskProcessor.Start();
			}
			else if (DownloadMaxCount <= GetDownloadingCount())
			{
				if (TaskDictionary[task.DownloadInfo].DownloadInfo.downloadStatus != DownloadStatus.DOWNLOADING && TaskDictionary[task.DownloadInfo].DownloadInfo.downloadStatus != DownloadStatus.SUCCESS)
				{
					DownloadTask downloadTask2 = FindLowerLevelTask();
					if (task.Controller.ControllerLevel > TaskDictionary[task.DownloadInfo].Controller.ControllerLevel)
					{
						TaskDictionary[task.DownloadInfo] = task;
					}
					if (downloadTask2 != null && TaskDictionary[task.DownloadInfo].Controller.ControllerLevel > downloadTask2.Controller.ControllerLevel)
					{
						downloadTask2.FireStopLowerControllerLevelTask();
						downloadTask2.downloadTaskProcessor.Stop();
					}
				}
				TaskDictionary[task.DownloadInfo].downloadTaskProcessor.Start();
			}
			else
			{
				TaskDictionary[task.DownloadInfo].downloadTaskProcessor.Start();
			}
		}
	}

	public void Stop(DownloadTask task)
	{
		lock (lo)
		{
			if (TaskDictionary.ContainsKey(task.DownloadInfo))
			{
				TaskDictionary[task.DownloadInfo].downloadTaskProcessor.Stop();
			}
		}
	}

	public void Delete(DownloadTask task)
	{
		lock (lo)
		{
			if (TaskDictionary.ContainsKey(task.DownloadInfo))
			{
				TaskDictionary[task.DownloadInfo].downloadTaskProcessor.Delete();
				TaskDictionary.TryRemove(task.DownloadInfo, out task);
			}
		}
	}

	public void Dispose()
	{
		lock (lo)
		{
			if (TaskDictionary != null)
			{
				TaskDictionary.Clear();
				TaskDictionary = null;
			}
			if (m_woker != null)
			{
				m_woker = null;
			}
		}
	}

	private int GetDownloadingCount()
	{
		lock (lo)
		{
			return TaskDictionary.Where((KeyValuePair<AbstractDownloadInfo, DownloadTask> n) => n.Value.DownloadInfo.downloadStatus == DownloadStatus.DOWNLOADING).Count();
		}
	}

	private void FireDownloadStatusChanged(object sender, DownloadEventArgs e)
	{
		lock (lo)
		{
			if (this.OnDownloadStatusChanged != null)
			{
				Delegate[] invocationList = this.OnDownloadStatusChanged.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((EventHandler<DownloadEventArgs>)invocationList[i]).BeginInvoke(sender, e, null, null);
				}
			}
			if ((e.downloadTask.DownloadInfo.downloadStatus != DownloadStatus.DOWNLOADING && e.downloadTask.DownloadInfo.downloadStatus != 0 && e.downloadTask.DownloadInfo.downloadStatus != DownloadStatus.PAUSE) || e.downloadTask.DownloadInfo.downloadStatus == DownloadStatus.SUCCESS)
			{
				TaskDictionary.TryRemove(e.downloadTask.DownloadInfo, out var _);
			}
			if (e.downloadTask.DownloadInfo.downloadStatus != DownloadStatus.DOWNLOADING)
			{
				StartNextTask();
			}
		}
	}

	private void StartNextTask()
	{
		try
		{
			IEnumerable<KeyValuePair<AbstractDownloadInfo, DownloadTask>> source = TaskDictionary.Where((KeyValuePair<AbstractDownloadInfo, DownloadTask> n) => n.Value.DownloadInfo.downloadStatus == DownloadStatus.WAITTING);
			if (source.Count() > 0)
			{
				(from n in source
					orderby n.Value.Controller.ControllerLevel descending, n.Value.DownloadInfo.downloadLevel descending, n.Value.StartTime descending
					select n).First().Value.downloadTaskProcessor.Start();
			}
		}
		catch
		{
		}
	}
}
