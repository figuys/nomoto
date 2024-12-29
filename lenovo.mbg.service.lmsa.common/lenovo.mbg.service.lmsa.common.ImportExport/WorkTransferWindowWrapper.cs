using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.common.ImportExport;

public class WorkTransferWindowWrapper
{
	protected TitleConfigViewModel SubResourceTitleConfig;

	protected TitleConfigViewModel SubResourceCompleteConfig;

	protected TitleConfigViewModel SubResourceTotalConfig;

	private static List<string> donotNeedShowFailedItems = new List<string> { "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", "{89D4DB68-4258-4002-8557-E65959C558B3}", "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}" };

	private volatile int mProgressVal;

	private volatile int mSuccessCount;

	private volatile int mFailCount;

	private volatile int mUndoCount;

	private volatile int mSubSuccessCount;

	private volatile int mSubFailCount;

	protected string ModelName;

	protected bool CanRetry;

	public IUserMsgControl Win { get; }

	public WorkTransferWindowViewModel Vm { get; set; }

	protected IMessageBox MessageBox { get; }

	private TransferDetailGroupViewModel CurrentDeatilsGroupViewModel { get; set; }

	public int ProgressMaxValue { get; set; }

	public string DeviceId { get; set; }

	public Action OnCancelCmmandFired
	{
		get
		{
			return Vm.OnCancelCmmandFired;
		}
		set
		{
			Vm.OnCancelCmmandFired = value;
		}
	}

	public Action<int> CloseWindowCallback { get; set; }

	public Action<bool> OnRetryCommandFired { get; set; }

	public Action CompletedCallback { get; set; }

	public WorkTransferWindowWrapper(IMessageBox MessageBox, string modelName, string titleFormat, string subTitle, bool canRetry = true)
	{
		ModelName = modelName;
		CanRetry = canRetry;
		this.MessageBox = MessageBox;
		TitleConfigViewModel titleConfig = new TitleConfigViewModel(titleFormat, modelName, "#2393B9");
		ObservableCollection<TitleConfigViewModel> belowTips = new ObservableCollection<TitleConfigViewModel>
		{
			SubResourceTitleConfig = new TitleConfigViewModel(string.Empty),
			new TitleConfigViewModel(" ("),
			SubResourceCompleteConfig = new TitleConfigViewModel("0", "#2393B9"),
			new TitleConfigViewModel("/"),
			SubResourceTotalConfig = new TitleConfigViewModel("0"),
			new TitleConfigViewModel(")")
		};
		Vm = new WorkTransferWindowViewModel(MessageBox, titleConfig, subTitle, belowTips);
		Vm.CanRetry = canRetry;
		WorkTransferWindowViewModel vm = Vm;
		vm.OnRetryCommandFired = (Action<bool>)Delegate.Combine(vm.OnRetryCommandFired, new Action<bool>(RetryCommandHandler));
		WorkTransferWindowViewModel vm2 = Vm;
		vm2.CloseWindowCallback = (Action<int>)Delegate.Combine(vm2.CloseWindowCallback, new Action<int>(CloseWindowCallbackHandler));
		Vm.IsRunning = true;
	}

	public void BeginProcess(Action<WorkTransferWindowWrapper> task)
	{
		Task.Run(delegate
		{
			task?.Invoke(this);
		});
	}

	public void SetSubProgressInfo(string resourceType, int subTotal)
	{
		mSubSuccessCount = 0;
		mSubFailCount = 0;
		SubResourceTitleConfig.Title = resourceType;
		SubResourceCompleteConfig.Title = "0";
		SubResourceTotalConfig.Title = subTotal.ToString();
		CurrentDeatilsGroupViewModel = new TransferDetailGroupViewModel(resourceType, resourceType, subTotal, subTotal);
		if (Vm.DetailGroupItems.Count >= 1)
		{
			Vm.DetailGroupItems[Vm.DetailGroupItems.Count - 1].IsComplete1 = Vm.DetailGroupItems[Vm.DetailGroupItems.Count - 1].SuccessCount == Vm.DetailGroupItems[Vm.DetailGroupItems.Count - 1].Count;
		}
		Vm.DetailGroupItems.Add(CurrentDeatilsGroupViewModel);
	}

	public void ChangeCurrentDeatilsGroupViewMode(string resourceType, int total)
	{
		CurrentDeatilsGroupViewModel = Vm.DetailGroupItems.FirstOrDefault((TransferDetailGroupViewModel n) => n.Id == resourceType);
		SubResourceTitleConfig.Title = resourceType;
		SubResourceCompleteConfig.Title = "0";
		SubResourceTotalConfig.Title = total.ToString();
	}

	public void AddSuccessCount(string path, int subCount)
	{
		mSuccessCount += subCount;
		mSubSuccessCount += subCount;
		SubResourceCompleteConfig.Title = mSubSuccessCount.ToString();
		CurrentDeatilsGroupViewModel.CurrentCount = mSubSuccessCount;
		UpdatePercent();
		SetStatusIfFinished();
	}

	public void AddFailCount(string resourceType, string id, string path, int failedCount, AppDataTransferHelper.BackupRestoreResult _type)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (!donotNeedShowFailedItems.Contains(resourceType))
			{
				if (_type == AppDataTransferHelper.BackupRestoreResult.Undo)
				{
					CurrentDeatilsGroupViewModel.AddMissedItem(failedCount, new TransferFailedItemViewModel
					{
						Id = id,
						Message = path
					});
				}
				else
				{
					CurrentDeatilsGroupViewModel.AddFailedItem(new TransferFailedItemViewModel
					{
						Id = id,
						Message = path
					}, failedCount);
				}
			}
			else
			{
				CurrentDeatilsGroupViewModel.AddFailedItem(null, failedCount);
			}
		});
		if (_type == AppDataTransferHelper.BackupRestoreResult.Undo)
		{
			mUndoCount += failedCount;
		}
		else
		{
			mFailCount += failedCount;
		}
		mSubFailCount += failedCount;
		UpdatePercent();
		SetStatusIfFinished();
	}

	public void RetrySuccess(string id, string path)
	{
		mSubSuccessCount = CurrentDeatilsGroupViewModel.CurrentCount;
		mSuccessCount++;
		mSubSuccessCount++;
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			CurrentDeatilsGroupViewModel.RemoveWhenSuccess(id, path);
		});
		SubResourceCompleteConfig.Title = mSubSuccessCount.ToString();
		CurrentDeatilsGroupViewModel.CurrentCount = mSubSuccessCount;
		CurrentDeatilsGroupViewModel.IsComplete1 = CurrentDeatilsGroupViewModel.SuccessCount == CurrentDeatilsGroupViewModel.Count;
		UpdatePercent();
		SetStatusIfFinished();
	}

	public void RetryFailed(int failedCount, AppDataTransferHelper.BackupRestoreResult _type)
	{
		if (_type == AppDataTransferHelper.BackupRestoreResult.Undo)
		{
			mUndoCount += failedCount;
		}
		else
		{
			mFailCount += failedCount;
		}
		CurrentDeatilsGroupViewModel.IsComplete1 = false;
		mSubFailCount += failedCount;
		UpdatePercent();
		SetStatusIfFinished();
	}

	public void UpdateRate(long readTotal, long total)
	{
		Application.Current.Dispatcher.Invoke(() => Vm.CurrentResourceRate = " (" + GlobalFun.ConvertLong2String(readTotal, "F0") + "/" + GlobalFun.ConvertLong2String(total, "F0") + ")");
	}

	public void UpdateRate(string reateTitle, long readTotal, long total)
	{
		Application.Current.Dispatcher.Invoke(() => Vm.CurrentResourceRate = reateTitle + " (" + GlobalFun.ConvertLong2String(readTotal, "F0") + "/" + GlobalFun.ConvertLong2String(total, "F0") + ")");
	}

	public void UpdateRetryRate(long readTotal, long total)
	{
		Application.Current.Dispatcher.Invoke(() => Vm.Rate = " (" + GlobalFun.ConvertLong2String(readTotal, "F0") + "/" + GlobalFun.ConvertLong2String(total, "F0") + ")");
	}

	public void UpdateResultTitle(string titleFormat, string foreground = "#005D7F")
	{
		Application.Current.Dispatcher.Invoke(() => Vm.ResultTitleConfig = new TitleConfigViewModel(titleFormat, ModelName, foreground));
	}

	public void Finish()
	{
		if (Vm.IsCancel)
		{
			return;
		}
		mFailCount = ProgressMaxValue - mSuccessCount - mUndoCount;
		CompletedCallback?.Invoke();
		bool canRetry = CanRetry && (mFailCount > 0 || mUndoCount > 0);
		if (Win != null)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				Win.GetMsgUi().Close();
			});
		}
		Visibility showNotes = Visibility.Hidden;
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice == null || masterDevice.PhysicalStatus == DevicePhysicalStateEx.Offline || masterDevice.SoftStatus == DeviceSoftStateEx.Offline)
		{
			showNotes = Visibility.Visible;
		}
		if (HostProxy.deviceManager.ConntectedDevices.Count((DeviceEx n) => n.Identifer == DeviceId) == 0)
		{
			canRetry = false;
		}
		Vm.Finish(mSuccessCount, mFailCount, mUndoCount, canRetry, showNotes);
	}

	public void StopAndCloseWindow()
	{
		OnCancelCmmandFired?.Invoke();
		CloseWindowCallback?.Invoke(-1);
		Application.Current.Dispatcher.Invoke(delegate
		{
			Win.GetMsgUi().Close();
		});
	}

	public void Init(int total)
	{
		ProgressMaxValue = total;
		mProgressVal = 0;
		mSuccessCount = 0;
		mFailCount = 0;
		mUndoCount = 0;
		mSubSuccessCount = 0;
		mSubFailCount = 0;
		Vm.TotalSuccess = 0;
		Vm.TotalUndo = 0;
		Vm.TotalFailed = 0;
		UpdatePercent();
	}

	private void RetryCommandHandler(bool status)
	{
		if (HostProxy.deviceManager.MasterDevice is TcpAndroidDevice)
		{
			OnRetryCommandFired?.Invoke(status);
		}
	}

	private void CloseWindowCallbackHandler(int data)
	{
		CloseWindowCallback?.Invoke(data);
		if (Win != null)
		{
			Win.GetMsgUi().Close();
		}
	}

	private void UpdatePercent()
	{
		mProgressVal = mSuccessCount + mFailCount + mUndoCount;
		double value = (double)mProgressVal * 100.0 / (double)ProgressMaxValue;
		value = Math.Round(value, 1);
		Vm.Percent = ((value > 100.0) ? 100.0 : value);
	}

	private void SetStatusIfFinished()
	{
		if (mProgressVal >= ProgressMaxValue)
		{
			CurrentDeatilsGroupViewModel.IsComplete1 = CurrentDeatilsGroupViewModel.SuccessCount == CurrentDeatilsGroupViewModel.Count;
			Finish();
		}
	}
}
