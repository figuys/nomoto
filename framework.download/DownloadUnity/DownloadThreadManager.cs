using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public class DownloadThreadManager
{
	private List<ThreadHandle> TaskThreadList;

	private static object syncLock = new object();

	private static readonly DownloadThreadManager m_instance = new DownloadThreadManager();

	public static DownloadThreadManager Instance => m_instance;

	private DownloadThreadManager()
	{
		Initialize();
	}

	private void Initialize()
	{
		TaskThreadList = new List<ThreadHandle>();
	}

	public ThreadHandle StartTask(Action task)
	{
		return StartTask(task, null);
	}

	public ThreadHandle StartTask(Action task, Action<TaskStatus> callback)
	{
		lock (syncLock)
		{
			ThreadHandle threadHandle = new ThreadHandle();
			Task.Factory.StartNew(task, threadHandle.tokenSource.Token);
			TaskThreadList.Add(threadHandle);
			return threadHandle;
		}
	}

	public void StopTask(ThreadHandle handle)
	{
		lock (syncLock)
		{
			ThreadHandle threadHandle = TaskThreadList.FirstOrDefault((ThreadHandle n) => n.ID == handle.ID);
			if (threadHandle.tokenSource != null)
			{
				threadHandle.tokenSource.Cancel();
				TaskThreadList.Remove(threadHandle);
			}
		}
	}

	public int GetAllDownloadingCount()
	{
		lock (syncLock)
		{
			return TaskThreadList.Count;
		}
	}
}
