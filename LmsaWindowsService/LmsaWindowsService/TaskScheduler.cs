using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService;

public class TaskScheduler
{
	private object lockerList = new object();

	private Timer m_schedulerTimer;

	private const int INTERVAL = 5000;

	public List<TaskWorker> TaskWorkList { get; private set; }

	public TaskScheduler()
	{
		TaskWorkList = new List<TaskWorker>();
		m_schedulerTimer = new Timer(TaskLoopCallback, null, 0, 5000);
	}

	public void Add(ITask task)
	{
		lock (lockerList)
		{
			TaskWorkList.Add(new TaskWorker(task));
		}
	}

	public void Remove(string taskName)
	{
		lock (lockerList)
		{
			TaskWorker taskWorker = TaskWorkList.FirstOrDefault((TaskWorker n) => n.Task.Name == taskName);
			if (taskWorker != null)
			{
				TaskWorkList.Remove(taskWorker);
			}
		}
	}

	private void TaskLoopCallback(object state)
	{
		lock (lockerList)
		{
			ClearTask();
			foreach (TaskWorker item in TaskWorkList.Where((TaskWorker n) => !n.Tiggered))
			{
				item.Start();
			}
			Thread.Sleep(30);
		}
	}

	private void ClearTask()
	{
		List<TaskWorker> list = TaskWorkList.Where((TaskWorker n) => n.Tiggered && !n.IsRunning)?.ToList();
		if (list != null && list.Count() > 0)
		{
			for (int i = 0; i < list.Count(); i++)
			{
				list[i].Dispose();
				TaskWorkList.Remove(list[i]);
			}
		}
	}
}
