using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class PicsThumbnailFileLoader : IDisposable
{
	private class TaskItem : IDisposable
	{
		private CancellationTokenSource _cancelTokenSource;

		private PicsThumbnailFileLoader _outer;

		public TaskItem(PicsThumbnailFileLoader outer)
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

	private DevicePicManagement dpm = new DevicePicManagement();

	private static PicsThumbnailFileLoader _nstance = null;

	private static object locksingle = new object();

	private readonly object _taskLock;

	private TaskItem _currentWorkingTask;

	public static PicsThumbnailFileLoader Instance
	{
		get
		{
			if (_nstance == null)
			{
				lock (locksingle)
				{
					if (_nstance == null)
					{
						_nstance = new PicsThumbnailFileLoader();
					}
				}
			}
			return _nstance;
		}
	}

	public bool IsDisposed { get; private set; }

	private PicsThumbnailFileLoader()
	{
		_taskLock = new object();
	}

	public void RecivePicListTask(string storageDir, List<PicInfoViewModel> pics, Action finiashedCallback)
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
				newTask.Start(new Tuple<string, List<PicInfoViewModel>>(storageDir, pics), TransferFile);
				if (newTask.Equals(Interlocked.CompareExchange(ref _currentWorkingTask, null, newTask)))
				{
					finiashedCallback?.Invoke();
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Start load pic task throw exception:" + ex.ToString());
			}
		});
	}

	public void ResetInitialize()
	{
		if (_currentWorkingTask != null)
		{
			_currentWorkingTask.Cancel();
		}
		_currentWorkingTask = null;
		lock (locksingle)
		{
			_nstance = new PicsThumbnailFileLoader();
		}
	}

	private void TransferFile(Tuple<string, List<PicInfoViewModel>> taskObj, CancellationTokenSource cancelTokenSource)
	{
		int count = 0;
		List<PicInfoViewModel> item = taskObj.Item2;
		string item2 = taskObj.Item1;
		LogHelper.LogInstance.Debug("TransferFile called!");
		List<ServerPicInfo> pics = (from m in item
			where m.RawPicInfo != null && string.IsNullOrEmpty(m.RawPicInfo.RawFilePath)
			select m.RawPicInfo).ToList();
		dpm.FillPicPath(ref pics);
		IEnumerable<PicInfoViewModel> unLoadImgVMArr = item.Where((PicInfoViewModel m) => m.RawPicInfo != null && !m.IsImageLoaded);
		dpm.ExportThumbnailFromDevice(null, unLoadImgVMArr.Select((PicInfoViewModel m) => m.RawPicInfo).ToList(), item2, delegate(ServerPicInfo pic, bool isExportOk)
		{
			if (isExportOk)
			{
				PicInfoViewModel imgVM = unLoadImgVMArr.Where((PicInfoViewModel m) => m.RawPicInfo == pic).FirstOrDefault();
				if (imgVM != null && pic != null && File.Exists(pic.LocalFilePath))
				{
					LogHelper.LogInstance.Debug($"export result: {pic.Id}\t{pic.VirtualFileName}\t{pic.LocalFilePath}\t{isExportOk}");
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						BitmapImage bitmapImage = new BitmapImage();
						try
						{
							bitmapImage.BeginInit();
							bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
							bitmapImage.UriSource = new Uri(pic.LocalFilePath);
							bitmapImage.EndInit();
						}
						catch (Exception)
						{
							bitmapImage = null;
						}
						imgVM.SetImage(bitmapImage);
						int num = count;
						count = num + 1;
					});
				}
			}
		}, cancelTokenSource);
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
