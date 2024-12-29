using System;
using System.Threading;
using System.Threading.Tasks;

namespace lenovo.themes.generic.Component;

public class SingleSyncTask : IDisposable
{
	private class TaskItem : IDisposable
	{
		private CancellationTokenSource _cancelTokenSource;

		private SingleSyncTask _outer;

		public TaskItem(SingleSyncTask outer)
		{
			_outer = outer;
			_cancelTokenSource = new CancellationTokenSource();
		}

		public void Start<T>(T taskParams, Action<T, CancellationTokenSource> taskHandler)
		{
			if (_cancelTokenSource.IsCancellationRequested)
			{
				return;
			}
			lock (_outer._taskLock)
			{
				if (!_cancelTokenSource.IsCancellationRequested)
				{
					taskHandler?.Invoke(taskParams, _cancelTokenSource);
				}
			}
		}

		public void Cancel()
		{
			_cancelTokenSource.Cancel();
		}

		public void Dispose()
		{
			if (_cancelTokenSource != null)
			{
				try
				{
					_cancelTokenSource.Dispose();
				}
				catch (Exception)
				{
				}
			}
		}
	}

	private readonly object _taskLock;

	private TaskItem _currentWorkingTask;

	public bool IsDisposed { get; private set; }

	public SingleSyncTask()
	{
		_taskLock = new object();
	}

	public void Load<T>(T targetData, Action<T, CancellationTokenSource> dataHandler, Action<bool, Exception> finiashedCallback)
	{
		TaskItem newTask = new TaskItem(this);
		TaskItem taskItem = Interlocked.Exchange(ref _currentWorkingTask, newTask);
		if (taskItem != null)
		{
			taskItem.Cancel();
			taskItem.Dispose();
		}
		Task.Factory.StartNew(delegate
		{
			try
			{
				newTask.Start(targetData, dataHandler);
				if (newTask.Equals(Interlocked.CompareExchange(ref _currentWorkingTask, null, newTask)))
				{
					finiashedCallback?.Invoke(arg1: true, null);
				}
				else
				{
					finiashedCallback?.Invoke(arg1: false, null);
				}
			}
			catch (Exception arg)
			{
				finiashedCallback?.Invoke(arg1: false, arg);
			}
		});
	}

	public void Dispose()
	{
		lock (this)
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				if (_currentWorkingTask != null)
				{
					_currentWorkingTask.Cancel();
					_currentWorkingTask.Dispose();
				}
			}
		}
	}
}
