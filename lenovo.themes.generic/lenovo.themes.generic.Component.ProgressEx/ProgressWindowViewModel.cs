using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Component.ProgressEx;

public class ProgressWindowViewModel : ViewModelBase
{
	public enum WorkStatus
	{
		Normal,
		Cancel,
		FinishWithFailedItems,
		FinishAll
	}

	public class DeatilsGroupViewModel : ViewModelBase
	{
		private ProgressTipsItemViewModel title;

		private bool isFailedItemsPanelClosed;

		private Visibility failedItemsPanelClosedVisibility = Visibility.Collapsed;

		private Visibility lineVisibility = Visibility.Collapsed;

		private int count;

		private int successCount;

		private int failCount;

		private bool haveFailItems;

		private ProgressTipsItemViewModel missingGroup;

		private Visibility missingGroupVisibility = Visibility.Collapsed;

		private ObservableCollection<FailedGroupSubItemViewModel> failedItems;

		public string Id { get; set; }

		public ProgressTipsItemViewModel DeatilTitle
		{
			get
			{
				return title;
			}
			set
			{
				if (title != value)
				{
					title = value;
					OnPropertyChanged("DeatilTitle");
				}
			}
		}

		public int MissingCount { get; set; }

		public bool IsFailedItemsPanelClosed
		{
			get
			{
				return isFailedItemsPanelClosed;
			}
			set
			{
				if (isFailedItemsPanelClosed != value)
				{
					isFailedItemsPanelClosed = value;
					OnPropertyChanged("IsFailedItemsPanelClosed");
				}
			}
		}

		public Visibility FailedItemsPanelClosedVisibility
		{
			get
			{
				return failedItemsPanelClosedVisibility;
			}
			set
			{
				if (failedItemsPanelClosedVisibility != value)
				{
					failedItemsPanelClosedVisibility = value;
					OnPropertyChanged("FailedItemsPanelClosedVisibility");
				}
			}
		}

		public Visibility LineVisibility
		{
			get
			{
				return lineVisibility;
			}
			set
			{
				if (lineVisibility != value)
				{
					lineVisibility = value;
					OnPropertyChanged("LineVisibility");
				}
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
			set
			{
				if (count != value)
				{
					count = value;
					OnPropertyChanged("Count");
				}
			}
		}

		public int SuccessCount
		{
			get
			{
				return successCount;
			}
			set
			{
				if (successCount != value)
				{
					successCount = value;
					OnPropertyChanged("SuccessCount");
				}
			}
		}

		public int FailCount
		{
			get
			{
				return failCount;
			}
			set
			{
				if (failCount != value)
				{
					failCount = value;
					OnPropertyChanged("FailCount");
				}
			}
		}

		public bool HaveFailItems
		{
			get
			{
				return haveFailItems;
			}
			set
			{
				if (haveFailItems != value)
				{
					haveFailItems = value;
					OnPropertyChanged("HaveFailItems");
				}
			}
		}

		public ProgressTipsItemViewModel MissingGroup
		{
			get
			{
				return missingGroup;
			}
			set
			{
				if (missingGroup != value)
				{
					missingGroup = value;
					OnPropertyChanged("MissingGroup");
				}
			}
		}

		public Visibility MissingGroupVisibility
		{
			get
			{
				return missingGroupVisibility;
			}
			set
			{
				if (missingGroupVisibility != value)
				{
					missingGroupVisibility = value;
					OnPropertyChanged("MissingGroupVisibility");
				}
			}
		}

		public ObservableCollection<FailedGroupSubItemViewModel> FailedItems
		{
			get
			{
				return failedItems;
			}
			set
			{
				if (failedItems != value)
				{
					failedItems = value;
					OnPropertyChanged("FailedItems");
				}
			}
		}

		public DeatilsGroupViewModel()
		{
			FailedItems = new ObservableCollection<FailedGroupSubItemViewModel>();
		}

		public void RemoveWhenSuccess(string id, string path)
		{
			FailCount--;
			SuccessCount++;
			if ("Missing".Equals(path))
			{
				MissingCount--;
			}
			FailedGroupSubItemViewModel failedGroupSubItemViewModel = FailedItems.FirstOrDefault((FailedGroupSubItemViewModel n) => n.Id == id);
			if (failedGroupSubItemViewModel != null)
			{
				FailedItems.Remove(failedGroupSubItemViewModel);
			}
		}

		public void AddFailedItem(FailedGroupSubItemViewModel item, int failedCount)
		{
			if (item != null)
			{
				FailedItemsPanelClosedVisibility = Visibility.Visible;
				FailedItems.Add(item);
			}
			FailCount += failedCount;
			SuccessCount -= failedCount;
		}

		public void AddMissedItem(int missedCount)
		{
			if (MissingCount == 0)
			{
				MissingGroup = new ProgressTipsItemViewModel
				{
					Message = "item(s) missed",
					FontSize = 13.0
				};
			}
			MissingCount += missedCount;
			FailCount += missedCount;
			SuccessCount -= missedCount;
			MissingGroupVisibility = Visibility.Visible;
			FailedItemsPanelClosedVisibility = Visibility.Visible;
		}
	}

	public class FailedGroupSubItemViewModel : ViewModelBase
	{
		private ProgressTipsItemViewModel failedItem;

		public string Id { get; set; }

		public ProgressTipsItemViewModel FailedItem
		{
			get
			{
				return failedItem;
			}
			set
			{
				if (failedItem != value)
				{
					failedItem = value;
					OnPropertyChanged("FailedItem");
				}
			}
		}
	}

	private Dispatcher mCurrentDispatcher;

	private HostMaskLayerWrapper mHostMaskLayerWrapper;

	private ProgressWindowViewModel contentBindingObj;

	private ProgressTipsItemViewModel title;

	private ObservableCollection<ProgressTipsItemViewModel> progressAboveTips;

	private ObservableCollection<ProgressTipsItemViewModel> progressBelowTips;

	private ObservableCollection<DeatilsGroupViewModel> failedGroupItems;

	private double percent;

	private string currentResourceRate;

	private string errorTips;

	private WorkStatus mCurrentWorkStatus;

	private Visibility m_retryProgressVisibile = Visibility.Collapsed;

	private Visibility m_RetryVisibile;

	private string m_TransferName;

	private int m_TransferCount;

	private int m_TransferTotal;

	private string rate;

	private int m_RetryMaxCount;

	private int m_RetryCount;

	private ReplayCommand showFailedListCommand;

	private bool mFailedListIsLoaded;

	private Visibility _warningNoticeVisibility;

	private Visibility detailButtonVisibility = Visibility.Collapsed;

	private ReplayCommand deatilsButtonClickCommand;

	private Visibility cancelButtonVisibility;

	private ReplayCommand cancleButtonClickCommand;

	private Visibility retryButtonVisibility = Visibility.Collapsed;

	private ReplayCommand retryButtonClickCommand;

	private Visibility okButtonVisibility = Visibility.Collapsed;

	private ReplayCommand okButtonClickCommand;

	public ProgressWindowViewModel ContentBindingObj
	{
		get
		{
			return contentBindingObj;
		}
		set
		{
			if (contentBindingObj != value)
			{
				contentBindingObj = value;
				OnPropertyChanged("ContentBindingObj");
			}
		}
	}

	public Action<bool> OnRetryCommandFired { get; set; }

	public bool CanRetry { get; set; }

	public bool CanShowDetailButton { get; set; }

	public Action ReplaceOldWindowRequest { get; set; }

	public ProgressTipsItemViewModel Title
	{
		get
		{
			return title;
		}
		set
		{
			if (title != value)
			{
				title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public ObservableCollection<ProgressTipsItemViewModel> ProgressAboveTips
	{
		get
		{
			return progressAboveTips;
		}
		set
		{
			if (progressAboveTips != value)
			{
				progressAboveTips = value;
				OnPropertyChanged("ProgressAboveTips");
			}
		}
	}

	public ObservableCollection<ProgressTipsItemViewModel> ProgressBelowTips
	{
		get
		{
			return progressBelowTips;
		}
		set
		{
			if (progressBelowTips != value)
			{
				progressBelowTips = value;
				OnPropertyChanged("ProgressBelowTips");
			}
		}
	}

	public ObservableCollection<DeatilsGroupViewModel> FailedGroupItems
	{
		get
		{
			return failedGroupItems;
		}
		set
		{
			if (failedGroupItems != value)
			{
				failedGroupItems = value;
				OnPropertyChanged("FailedGroupItems");
			}
		}
	}

	public double Percent
	{
		get
		{
			return percent;
		}
		set
		{
			if (percent != value)
			{
				percent = value;
				OnPropertyChanged("Percent");
			}
		}
	}

	public long ProgressMaxVal { get; set; }

	public string CurrentResourceRate
	{
		get
		{
			return currentResourceRate;
		}
		set
		{
			if (!(currentResourceRate == value))
			{
				currentResourceRate = value;
				OnPropertyChanged("CurrentResourceRate");
			}
		}
	}

	public string ErrorTips
	{
		get
		{
			return errorTips;
		}
		set
		{
			if (!(errorTips == value))
			{
				errorTips = value;
				OnPropertyChanged("ErrorTips");
			}
		}
	}

	public WorkStatus CurrentWorkStatus
	{
		get
		{
			return mCurrentWorkStatus;
		}
		set
		{
			if (mCurrentWorkStatus != value)
			{
				mCurrentWorkStatus = value;
				OnPropertyChanged("CurrentWorkStatus");
			}
		}
	}

	public Visibility RetryProgressVisibile
	{
		get
		{
			return m_retryProgressVisibile;
		}
		set
		{
			m_retryProgressVisibile = value;
			if (m_retryProgressVisibile == Visibility.Collapsed)
			{
				RetryVisibile = Visibility.Visible;
			}
			else
			{
				RetryVisibile = Visibility.Collapsed;
			}
			OnPropertyChanged("RetryProgressVisibile");
		}
	}

	public Visibility RetryVisibile
	{
		get
		{
			return m_RetryVisibile;
		}
		set
		{
			m_RetryVisibile = value;
			OnPropertyChanged("RetryVisibile");
		}
	}

	public string TransferName
	{
		get
		{
			return m_TransferName;
		}
		set
		{
			m_TransferName = value;
			OnPropertyChanged("TransferName");
		}
	}

	public int TransferCount
	{
		get
		{
			return m_TransferCount;
		}
		set
		{
			m_TransferCount = value;
			if (m_TransferCount > 0)
			{
				RetryCount++;
			}
			OnPropertyChanged("TransferCount");
		}
	}

	public int TransferTotal
	{
		get
		{
			return m_TransferTotal;
		}
		set
		{
			m_TransferTotal = value;
			OnPropertyChanged("TransferTotal");
		}
	}

	public string Rate
	{
		get
		{
			return rate;
		}
		set
		{
			if (!(rate == value))
			{
				rate = value;
				OnPropertyChanged("Rate");
			}
		}
	}

	public int RetryMaxCount
	{
		get
		{
			return m_RetryMaxCount;
		}
		set
		{
			m_RetryMaxCount = value;
			OnPropertyChanged("RetryMaxCount");
		}
	}

	public int RetryCount
	{
		get
		{
			return m_RetryCount;
		}
		set
		{
			m_RetryCount = value;
			if (RetryCount >= RetryMaxCount)
			{
				RetryProgressVisibile = Visibility.Collapsed;
			}
			OnPropertyChanged("RetryCount");
		}
	}

	public Action<int> CloseWindowCallback { get; set; }

	public ReplayCommand ShowLoadFailedListCommand
	{
		get
		{
			return showFailedListCommand;
		}
		set
		{
			if (showFailedListCommand != value)
			{
				showFailedListCommand = value;
				OnPropertyChanged("ShowLoadFailedListCommand");
			}
		}
	}

	public Visibility WarningNoticeVisibility
	{
		get
		{
			return _warningNoticeVisibility;
		}
		set
		{
			if (_warningNoticeVisibility != value)
			{
				_warningNoticeVisibility = value;
				OnPropertyChanged("WarningNoticeVisibility");
			}
		}
	}

	public Visibility DetailButtonVisibility
	{
		get
		{
			return detailButtonVisibility;
		}
		set
		{
			if (detailButtonVisibility != value)
			{
				detailButtonVisibility = value;
				OnPropertyChanged("DetailButtonVisibility");
			}
		}
	}

	public ReplayCommand DeatilsButtonClickCommand
	{
		get
		{
			return deatilsButtonClickCommand;
		}
		set
		{
			if (deatilsButtonClickCommand != value)
			{
				deatilsButtonClickCommand = value;
				OnPropertyChanged("DeatilsButtonClickCommand");
			}
		}
	}

	public Visibility CancelButtonVisibility
	{
		get
		{
			return cancelButtonVisibility;
		}
		set
		{
			if (cancelButtonVisibility != value)
			{
				cancelButtonVisibility = value;
				OnPropertyChanged("CancelButtonVisibility");
			}
		}
	}

	public ReplayCommand CancelButtonClickCommand
	{
		get
		{
			return cancleButtonClickCommand;
		}
		set
		{
			if (cancleButtonClickCommand != value)
			{
				cancleButtonClickCommand = value;
				OnPropertyChanged("CancelButtonClickCommand");
			}
		}
	}

	public Action OnCancelCmmandFired { get; set; }

	public Visibility RetryButtonVisibility
	{
		get
		{
			return retryButtonVisibility;
		}
		set
		{
			if (retryButtonVisibility != value)
			{
				retryButtonVisibility = value;
				OnPropertyChanged("RetryButtonVisibility");
			}
		}
	}

	public ReplayCommand RetryButtonCLickCommand
	{
		get
		{
			return retryButtonClickCommand;
		}
		set
		{
			if (retryButtonClickCommand != value)
			{
				retryButtonClickCommand = value;
				OnPropertyChanged("RetryButtonCLickCommand");
			}
		}
	}

	public Visibility OkButtonVisibility
	{
		get
		{
			return okButtonVisibility;
		}
		set
		{
			if (okButtonVisibility != value)
			{
				okButtonVisibility = value;
				OnPropertyChanged("OkButtonVisibility");
			}
		}
	}

	public ReplayCommand RetryCancelClickCommand { get; private set; }

	public ReplayCommand OkButtonClickCommand
	{
		get
		{
			return okButtonClickCommand;
		}
		set
		{
			if (okButtonClickCommand != value)
			{
				okButtonClickCommand = value;
				OnPropertyChanged("OkButtonClickCommand");
			}
		}
	}

	public ProgressWindowViewModel()
	{
		FailedGroupItems = new ObservableCollection<DeatilsGroupViewModel>();
		ShowLoadFailedListCommand = new ReplayCommand(ShowLoadFailedListCommandHandler);
		RetryButtonCLickCommand = new ReplayCommand(RetryButtonCLickCommandHandler);
		OkButtonClickCommand = new ReplayCommand(OkButtonClickCommandHandler);
		RetryCancelClickCommand = new ReplayCommand(RetryCancelClickCommandHandler);
		CancelButtonClickCommand = new ReplayCommand(CancelButtonClickCommandHandlerV6);
		CanRetry = false;
		CanShowDetailButton = true;
	}

	public ProgressWindowViewModel(Dispatcher currentDispatcher, HostMaskLayerWrapper hostMaskLayerWrapper)
		: this()
	{
		mCurrentDispatcher = currentDispatcher;
		mHostMaskLayerWrapper = hostMaskLayerWrapper;
	}

	public void Finish(WorkStatus status)
	{
		CurrentWorkStatus = status;
		RetryButtonVisibility = Visibility.Collapsed;
		switch (status)
		{
		case WorkStatus.FinishWithFailedItems:
			WarningNoticeVisibility = Visibility.Hidden;
			if (CanShowDetailButton)
			{
				DetailButtonVisibility = Visibility.Visible;
				CancelButtonVisibility = Visibility.Collapsed;
				OkButtonVisibility = Visibility.Collapsed;
			}
			else
			{
				DetailButtonVisibility = Visibility.Collapsed;
				CancelButtonVisibility = Visibility.Collapsed;
				OkButtonVisibility = Visibility.Visible;
			}
			if (CanRetry)
			{
				RetryButtonVisibility = Visibility.Visible;
			}
			break;
		case WorkStatus.FinishAll:
			WarningNoticeVisibility = Visibility.Hidden;
			DetailButtonVisibility = Visibility.Collapsed;
			CancelButtonVisibility = Visibility.Collapsed;
			OkButtonVisibility = Visibility.Visible;
			break;
		case WorkStatus.Normal:
		case WorkStatus.Cancel:
			break;
		}
	}

	private void ShowLoadFailedListCommandHandler(object args)
	{
		if (!mFailedListIsLoaded)
		{
			mFailedListIsLoaded = true;
		}
	}

	public void RetryCancelClickCommandHandler(object args)
	{
		OkButtonClickCommandHandler(args);
	}

	private void CancelButtonClickCommandHandler(object args)
	{
		if (!(Percent < 1.0))
		{
			return;
		}
		LenovoPopupWindow _okCancelWindow = new OkCancelWindowModel().CreateWindow("K0208", "K0541", "K0208", "K0327");
		mHostMaskLayerWrapper.New(_okCancelWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			mCurrentDispatcher.Invoke(delegate
			{
				_okCancelWindow.ShowDialog();
			});
		});
		if (_okCancelWindow.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
		{
			OnCancelCmmandFired?.Invoke();
			Window obj = args as Window;
			CloseWindowCallback?.Invoke(-1);
			obj.Close();
		}
	}

	private void CancelButtonClickCommandHandlerV6(object args)
	{
		if (Percent < 1.0 && MessageBoxEx.Show(mHostMaskLayerWrapper, "K0541") == true)
		{
			OnCancelCmmandFired?.Invoke();
			Window obj = args as Window;
			CloseWindowCallback?.Invoke(-1);
			obj.Close();
		}
	}

	private void RetryButtonCLickCommandHandler(object args)
	{
		OnRetryCommandFired?.Invoke(obj: false);
	}

	private void OkButtonClickCommandHandler(object args)
	{
		Window obj = args as Window;
		CloseWindowCallback?.Invoke(0);
		obj.Close();
	}
}
