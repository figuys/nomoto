using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Component;
using lenovo.themes.generic.Component.ProgressEx;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.common.ImportExport;

public class ProgressWindowWrapper
{
	private ProgressWindowViewModel mViewModel;

	private ObservableCollection<ProgressTipsItemViewModel> mAboveTips;

	private ObservableCollection<ProgressTipsItemViewModel> mBelowTips;

	private int mProgressVal;

	private ProgressWindowV6 mWin;

	private HostMaskLayerWrapper mHostMaskLayerWrapper;

	private ProgressTipsItemViewModel mSubResourceCompleteMessageItemViewModel;

	private ProgressTipsItemViewModel mSubResourceTotalMessageItemViewModel;

	private ProgressTipsItemViewModel mSubResourceTitleMessageItemViewModel;

	private volatile bool mIsAutoCheckFinishStatus;

	private string mCurrentSubResourceType = string.Empty;

	private ProgressResult resultWin;

	private Action<int> mCloseWindowCallback;

	private volatile int mSuccessCount;

	private volatile int mFailCount;

	private volatile int mSubSuccessCount;

	private volatile int mSubFailCount;

	private double mbSize = 1048576.0;

	private bool firstGroup = true;

	private static List<string> donotNeedShowFailedItems = new List<string> { "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", "{89D4DB68-4258-4002-8557-E65959C558B3}", "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}" };

	public ProgressWindowViewModel viewModel => mViewModel;

	public ObservableCollection<ProgressTipsItemViewModel> AboveTips => mAboveTips;

	public ObservableCollection<ProgressTipsItemViewModel> BelowTips => mBelowTips;

	private ProgressWindowViewModel.DeatilsGroupViewModel CurrentDeatilsGroupViewModel { get; set; }

	public Action<bool> OnRetryCommandFired
	{
		get
		{
			return mViewModel.OnRetryCommandFired;
		}
		set
		{
			mViewModel.OnRetryCommandFired = value;
		}
	}

	public Action OnCancelCmmandFired
	{
		get
		{
			return mViewModel.OnCancelCmmandFired;
		}
		set
		{
			mViewModel.OnCancelCmmandFired = value;
		}
	}

	public Action<int> CloseWindowCallback
	{
		get
		{
			return mCloseWindowCallback;
		}
		set
		{
			mCloseWindowCallback = value;
			mViewModel.CloseWindowCallback = delegate(int code)
			{
				if (mCloseWindowCallback != null)
				{
					try
					{
						mCloseWindowCallback(code);
					}
					catch (Exception)
					{
					}
				}
			};
		}
	}

	public int ProgressMaxValue { get; set; }

	public Action CompletedCallback { get; set; }

	public int SuccessCount => mSuccessCount;

	public int FailCount => mFailCount;

	private void DeatilsButtonClickCommandHandler(object args)
	{
		mWin.Close();
		resultWin = new ProgressResult();
		resultWin.DataContext = mViewModel;
		mHostMaskLayerWrapper.New(resultWin, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			resultWin.ShowDialog();
		});
	}

	public void CloseResultWindow()
	{
		resultWin?.Close();
		resultWin = null;
	}

	public ProgressWindowWrapper(ProgressTipsItemViewModel title, bool canRetry, bool canShowDeatil)
	{
		mIsAutoCheckFinishStatus = true;
		mWin = new ProgressWindowV6();
		mWin.Uid = "MMAA";
		mWin.Width = 600.0;
		mHostMaskLayerWrapper = HostProxy.HostMaskLayerWrapper;
		mViewModel = new ProgressWindowViewModel(mWin.Dispatcher, HostProxy.HostMaskLayerWrapper);
		mViewModel.CanRetry = canRetry;
		mViewModel.CanShowDetailButton = false;
		mAboveTips = new ObservableCollection<ProgressTipsItemViewModel> { title };
		mViewModel.ProgressAboveTips = mAboveTips;
		ObservableCollection<ProgressTipsItemViewModel> observableCollection = new ObservableCollection<ProgressTipsItemViewModel>();
		ProgressTipsItemViewModel obj = new ProgressTipsItemViewModel
		{
			Message = string.Empty
		};
		ProgressTipsItemViewModel item = obj;
		mSubResourceTitleMessageItemViewModel = obj;
		observableCollection.Add(item);
		observableCollection.Add(new ProgressTipsItemViewModel
		{
			Message = " ("
		});
		ProgressTipsItemViewModel obj2 = new ProgressTipsItemViewModel
		{
			Message = "0",
			Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005D7F"))
		};
		item = obj2;
		mSubResourceCompleteMessageItemViewModel = obj2;
		observableCollection.Add(item);
		observableCollection.Add(new ProgressTipsItemViewModel
		{
			Message = "/"
		});
		ProgressTipsItemViewModel obj3 = new ProgressTipsItemViewModel
		{
			Message = "0"
		};
		item = obj3;
		mSubResourceTotalMessageItemViewModel = obj3;
		observableCollection.Add(item);
		observableCollection.Add(new ProgressTipsItemViewModel
		{
			Message = ")"
		});
		mBelowTips = observableCollection;
		mViewModel.ProgressBelowTips = mBelowTips;
		mViewModel.DeatilsButtonClickCommand = new ReplayCommand(DeatilsButtonClickCommandHandler);
		mWin.DataContext = mViewModel;
		mViewModel.ContentBindingObj = mViewModel;
	}

	public void BeginProcess(Action<ProgressWindowWrapper> task)
	{
		Task.Factory.StartNew(delegate
		{
			task(this);
		});
		mHostMaskLayerWrapper.New(mWin, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			mWin.ShowDialog();
		});
	}

	public void UpdateRate(long readTotal, long total)
	{
		mViewModel.CurrentResourceRate = " (" + ConvertBytesWithUnit(readTotal) + "/" + ConvertBytesWithUnit(total) + ")";
	}

	public void UpdateRate(string reateTitle, long readTotal, long total)
	{
		mViewModel.CurrentResourceRate = reateTitle + " (" + ConvertBytesWithUnit(readTotal) + "/" + ConvertBytesWithUnit(total) + ")";
	}

	public void UpdateRetryRate(long readTotal, long total)
	{
		mViewModel.Rate = " (" + ConvertBytesWithUnit(readTotal) + "/" + ConvertBytesWithUnit(total) + ")";
	}

	private string ConvertBytesWithUnit(long size)
	{
		if ((double)size >= mbSize)
		{
			return Math.Round((double)size / mbSize, 2) + " MB";
		}
		if (size >= 1024)
		{
			return Math.Round((double)size / 1024.0, 2) + " KB";
		}
		return size + " Bytes";
	}

	public void SetSubProgressInfo(string resourceType, int subTotal)
	{
		mSubResourceTitleMessageItemViewModel.Message = resourceType;
		mCurrentSubResourceType = resourceType;
		mSubSuccessCount = 0;
		mSubFailCount = 0;
		mSubResourceCompleteMessageItemViewModel.Message = "0";
		mSubResourceTotalMessageItemViewModel.Message = subTotal.ToString();
		CurrentDeatilsGroupViewModel = new ProgressWindowViewModel.DeatilsGroupViewModel();
		if (firstGroup)
		{
			CurrentDeatilsGroupViewModel.LineVisibility = Visibility.Collapsed;
			CurrentDeatilsGroupViewModel.IsFailedItemsPanelClosed = false;
		}
		else
		{
			CurrentDeatilsGroupViewModel.LineVisibility = Visibility.Visible;
			CurrentDeatilsGroupViewModel.IsFailedItemsPanelClosed = true;
		}
		firstGroup = false;
		CurrentDeatilsGroupViewModel.Id = resourceType;
		CurrentDeatilsGroupViewModel.Count = subTotal;
		CurrentDeatilsGroupViewModel.SuccessCount = subTotal;
		CurrentDeatilsGroupViewModel.DeatilTitle = new ProgressTipsItemViewModel
		{
			Message = mCurrentSubResourceType,
			FontSize = 25.0
		};
		CurrentDeatilsGroupViewModel.FailedItems = new ObservableCollection<ProgressWindowViewModel.FailedGroupSubItemViewModel>();
		mViewModel.FailedGroupItems.Add(CurrentDeatilsGroupViewModel);
	}

	public void ChangeCurrentDeatilsGroupViewMode(string resourceType)
	{
		CurrentDeatilsGroupViewModel = mViewModel.FailedGroupItems.FirstOrDefault((ProgressWindowViewModel.DeatilsGroupViewModel n) => n.Id == resourceType);
	}

	public void AddSuccessCount(string path, int subCount)
	{
		mSuccessCount += subCount;
		mSubSuccessCount += subCount;
		mViewModel.Percent = (double)(int)((double)(mProgressVal = mSuccessCount + mFailCount) / (double)ProgressMaxValue * 100.0) / 100.0;
		mSubResourceCompleteMessageItemViewModel.Message = mSubSuccessCount.ToString();
		if (mIsAutoCheckFinishStatus)
		{
			SetStatusIfFinished();
		}
	}

	public void AddFailCount(string resourceType, string id, string path, int failedCount)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (!donotNeedShowFailedItems.Contains(resourceType))
			{
				mViewModel.CanShowDetailButton = true;
				if ("Missing".Equals(path))
				{
					CurrentDeatilsGroupViewModel.AddMissedItem(failedCount);
				}
				else
				{
					CurrentDeatilsGroupViewModel.AddFailedItem(new ProgressWindowViewModel.FailedGroupSubItemViewModel
					{
						Id = id,
						FailedItem = new ProgressTipsItemViewModel
						{
							Message = path,
							FontSize = 13.0
						}
					}, failedCount);
				}
			}
			else
			{
				CurrentDeatilsGroupViewModel.AddFailedItem(null, failedCount);
			}
		});
		mFailCount += failedCount;
		mSubFailCount += failedCount;
		mViewModel.Percent = (double)(mProgressVal = mSuccessCount + mFailCount) / (double)ProgressMaxValue;
		if (mIsAutoCheckFinishStatus)
		{
			SetStatusIfFinished();
		}
	}

	public void RemoveWhenSuccess(string id, string path)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			mSuccessCount++;
			mFailCount--;
			CurrentDeatilsGroupViewModel?.RemoveWhenSuccess(id, path);
		});
		if (mIsAutoCheckFinishStatus)
		{
			SetStatusIfFinished();
		}
	}

	public void Finish()
	{
		mFailCount = ProgressMaxValue - mSuccessCount;
		ProgressWindowViewModel.WorkStatus status = ((mFailCount == 0) ? ProgressWindowViewModel.WorkStatus.FinishAll : ProgressWindowViewModel.WorkStatus.FinishWithFailedItems);
		mViewModel.Finish(status);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			mViewModel.CurrentResourceRate = string.Empty;
			mAboveTips.Clear();
			CompletedCallback?.Invoke();
		});
	}

	private void SetStatusIfFinished()
	{
		if (mProgressVal >= ProgressMaxValue)
		{
			Finish();
		}
	}

	public void StopAndCloseWindow()
	{
		OnCancelCmmandFired?.Invoke();
		CloseWindowCallback?.Invoke(-1);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			mWin?.Close();
		});
	}
}
