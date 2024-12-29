using System;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DataPartPageLoader : IDisposable
{
	private class TaskItem : IDisposable
	{
		private CancellationTokenSource _cancelTokenSource;

		private DataPartPageLoader _outer;

		public TaskItem(DataPartPageLoader outer)
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

	public DataPartPageLoader()
	{
		_taskLock = new object();
	}

	public void Load<T>(T targetData, Action<T, CancellationTokenSource> dataHandler, Action finiashedCallback)
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
					finiashedCallback?.Invoke();
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Start load task throw exception:" + ex.ToString());
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
