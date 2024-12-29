using System;
using System.Collections.Generic;
using System.Linq;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService;

public static class TaskManager
{
	private static TaskScheduler Scheduler = new TaskScheduler();

	public static List<ITask> GetRunningTask()
	{
		return GetAllTask()?.Where((ITask n) => n.IsRunning)?.ToList();
	}

	public static List<ITask> GetAllTask()
	{
		return Scheduler.TaskWorkList.Select((TaskWorker n) => n.Task)?.ToList();
	}

	public static ITask GetTaskByName(string name)
	{
		return GetAllTask()?.FirstOrDefault((ITask n) => n.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
	}

	public static void RegisterTask(ITask worker)
	{
		Scheduler.Add(worker);
	}

	public static void DeRegisterScheduler(string taskName)
	{
		Scheduler.Remove(taskName);
	}

	public static void Dispose()
	{
		if (Scheduler == null)
		{
			return;
		}
		foreach (TaskWorker taskWork in Scheduler.TaskWorkList)
		{
			taskWork.Dispose();
		}
	}
}
