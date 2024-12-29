using System;
using System.Collections.ObjectModel;
using System.Windows;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.ViewModelV6;

public class WorkTransferWindowViewModel : ViewModelBase
{
	private TitleConfigViewModel _ResultTitleConfig;

	private bool _CanRetry;

	private bool? _IsRunning;

	private ObservableCollection<TransferDetailGroupViewModel> _DetailGroupItems;

	private double _Percent;

	private int _TotalSuccess;

	private int _TotalFailed;

	private int _totalUndo;

	private string _CurrentResourceRate;

	private string _Rate;

	private string _ErrorTips;

	private bool _HasFailed;

	private bool _hasUndo;

	private Visibility _textNotesVisibility = Visibility.Collapsed;

	public ReplayCommand CancelCommand { get; }

	public ReplayCommand RetryCommand { get; }

	public ReplayCommand OkCommand { get; }

	protected IMessageBox MessageBox { get; }

	public Action OnCancelCmmandFired { get; set; }

	public Action<int> CloseWindowCallback { get; set; }

	public Action<bool> OnRetryCommandFired { get; set; }

	public TitleConfigViewModel TitleConfig { get; set; }

	public TitleConfigViewModel ResultTitleConfig
	{
		get
		{
			return _ResultTitleConfig;
		}
		set
		{
			_ResultTitleConfig = value;
			OnPropertyChanged("ResultTitleConfig");
		}
	}

	public bool IsCancel { get; private set; }

	public bool CanRetry
	{
		get
		{
			return _CanRetry;
		}
		set
		{
			_CanRetry = value;
			OnPropertyChanged("CanRetry");
		}
	}

	public bool? IsRunning
	{
		get
		{
			return _IsRunning;
		}
		set
		{
			_IsRunning = value;
			OnPropertyChanged("IsRunning");
		}
	}

	public ObservableCollection<TitleConfigViewModel> ProgressBelowTips { get; set; }

	public ObservableCollection<TransferDetailGroupViewModel> DetailGroupItems
	{
		get
		{
			return _DetailGroupItems;
		}
		set
		{
			_DetailGroupItems = value;
			OnPropertyChanged("DetailGroupItems");
		}
	}

	public string SubTitle { get; set; }

	public double Percent
	{
		get
		{
			return _Percent;
		}
		set
		{
			_Percent = value;
			OnPropertyChanged("Percent");
		}
	}

	public int TotalSuccess
	{
		get
		{
			return _TotalSuccess;
		}
		set
		{
			_TotalSuccess = value;
			OnPropertyChanged("TotalSuccess");
		}
	}

	public int TotalFailed
	{
		get
		{
			return _TotalFailed;
		}
		set
		{
			_TotalFailed = value;
			if (_TotalFailed > 0)
			{
				HasFailed = true;
			}
			OnPropertyChanged("TotalFailed");
		}
	}

	public int TotalUndo
	{
		get
		{
			return _totalUndo;
		}
		set
		{
			_totalUndo = value;
			if (_totalUndo > 0)
			{
				HasUndo = true;
			}
			OnPropertyChanged("TotalUndo");
		}
	}

	public string CurrentResourceRate
	{
		get
		{
			return _CurrentResourceRate;
		}
		set
		{
			_CurrentResourceRate = value.Replace("\r\n", "").Replace("\n", "");
			OnPropertyChanged("CurrentResourceRate");
		}
	}

	public string Rate
	{
		get
		{
			return _Rate;
		}
		set
		{
			_Rate = value;
			OnPropertyChanged("Rate");
		}
	}

	public string ErrorTips
	{
		get
		{
			return _ErrorTips;
		}
		set
		{
			_ErrorTips = value;
			OnPropertyChanged("ErrorTips");
		}
	}

	public bool HasFailed
	{
		get
		{
			return _HasFailed;
		}
		set
		{
			_HasFailed = value;
			OnPropertyChanged("HasFailed");
		}
	}

	public bool HasUndo
	{
		get
		{
			return _hasUndo;
		}
		set
		{
			_hasUndo = value;
			OnPropertyChanged("HasUndo");
		}
	}

	public Visibility TextNotesVisibility
	{
		get
		{
			return _textNotesVisibility;
		}
		set
		{
			_textNotesVisibility = value;
			OnPropertyChanged("TextNotesVisibility");
		}
	}

	public WorkTransferWindowViewModel(IMessageBox MessageBox, TitleConfigViewModel titleConfig, string subTitle, ObservableCollection<TitleConfigViewModel> belowTips)
	{
		CanRetry = false;
		IsRunning = null;
		TitleConfig = titleConfig;
		SubTitle = subTitle;
		this.MessageBox = MessageBox;
		ProgressBelowTips = belowTips;
		DetailGroupItems = new ObservableCollection<TransferDetailGroupViewModel>();
		CancelCommand = new ReplayCommand(CancelCommandHandler);
		RetryCommand = new ReplayCommand(RetryCommandHandler);
		OkCommand = new ReplayCommand(OkCommandHandler);
	}

	public void Finish(int success, int failed, int undo, bool canRetry, Visibility _showNotes)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			TotalSuccess += success;
			TotalFailed = failed;
			TotalUndo = undo;
			CanRetry = canRetry;
			TextNotesVisibility = _showNotes;
			IsRunning = false;
		});
	}

	private void CancelCommandHandler(object data)
	{
		if (MessageBox.ShowMessage("K0541", MessageBoxButton.OKCancel) == true)
		{
			IsCancel = true;
			OnCancelCmmandFired?.Invoke();
			IsRunning = null;
			CloseWindowCallback?.Invoke(-1);
		}
	}

	private void RetryCommandHandler(object data)
	{
		IsRunning = true;
		OnRetryCommandFired?.Invoke(obj: false);
	}

	private void OkCommandHandler(object data)
	{
		IsRunning = null;
		CloseWindowCallback?.Invoke(1);
	}
}
