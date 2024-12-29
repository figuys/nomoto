using System;
using System.Threading.Tasks;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService;

public class TaskWorker : IDisposable
{
	private bool m_isDisposed;

	public ITask Task { get; private set; }

	public bool IsRunning => Task.IsRunning;

	public bool Tiggered { get; private set; }

	public TaskWorker(ITask task)
	{
		Task = task;
	}

	public void Start()
	{
		System.Threading.Tasks.Task.Factory.StartNew(delegate
		{
			Tiggered = true;
			Task.Start();
		});
	}

	public void Stop()
	{
		Task.Stop();
	}

	public void Dispose()
	{
		if (!m_isDisposed)
		{
			if (Task != null)
			{
				Task.Dispose();
				Task = null;
			}
			m_isDisposed = true;
		}
	}
}
