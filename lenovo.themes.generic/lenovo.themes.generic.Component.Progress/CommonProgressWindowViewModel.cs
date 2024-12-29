using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Component.Progress;

public class CommonProgressWindowViewModel : ViewModelBase
{
	private Dispatcher mCurrentDispatcher;

	private HostMaskLayerWrapper mHostMaskLayerWrapper;

	protected IMessageBox MessageBox;

	private double _Percent;

	protected string _ButtonContent = "K0208";

	protected string _Title;

	private WorkStatus _WorkStatus;

	private string currentResourceRate;

	public ObservableCollection<ObservableCollection<ProgressViewModel>> MessageSources { get; set; }

	public ObservableCollection<ProgressViewModel> Contents { get; set; }

	public ReplayCommand ClickCommand { get; private set; }

	protected IAsyncTaskCancelHander _CancelHandler { get; set; }

	public bool IsCompleted { get; set; }

	public int Total { get; set; }

	public int Completed { get; private set; }

	public int TotalCompleted { get; private set; }

	public int Failed { get; set; }

	public Action<object> CloseWindowCallback { get; private set; }

	public double Percent
	{
		get
		{
			return _Percent;
		}
		set
		{
			if (_Percent != value)
			{
				_Percent = value;
				OnPropertyChanged("Percent");
			}
		}
	}

	public virtual string ButtonContent
	{
		get
		{
			return _ButtonContent;
		}
		set
		{
			_ButtonContent = value;
			OnPropertyChanged("ButtonContent");
		}
	}

	public virtual string Title
	{
		get
		{
			return _Title;
		}
		set
		{
			_Title = value;
			OnPropertyChanged("Title");
		}
	}

	public virtual WorkStatus WorkStatus
	{
		get
		{
			return _WorkStatus;
		}
		set
		{
			if (_WorkStatus != value)
			{
				_WorkStatus = value;
				if (_WorkStatus == WorkStatus.Normal)
				{
					ButtonContent = "K0208";
				}
				else if (_WorkStatus == WorkStatus.Success)
				{
					ButtonContent = "K0386";
				}
				else if (_WorkStatus == WorkStatus.Failed)
				{
					ButtonContent = "K0337";
				}
				OnPropertyChanged("WorkStatus");
			}
		}
	}

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

	public CommonProgressWindowViewModel(IMessageBox messageBox, Dispatcher dispatcher, HostMaskLayerWrapper hostMaskLayerWrapper, Action<object> callback)
	{
		MessageBox = messageBox;
		MessageSources = new ObservableCollection<ObservableCollection<ProgressViewModel>>();
		Contents = new ObservableCollection<ProgressViewModel>();
		ClickCommand = new ReplayCommand(ClickCommandHandler);
		CloseWindowCallback = callback;
		mCurrentDispatcher = dispatcher;
		mHostMaskLayerWrapper = hostMaskLayerWrapper;
	}

	public virtual void Initilize(int total)
	{
		Initilize(total, null);
	}

	public virtual void Initilize(int total, List<ProgressViewModel> contents)
	{
		SetTotal(total);
		SetContents(contents);
	}

	public void SetCancelHandler(IAsyncTaskCancelHander cancelHandler)
	{
		_CancelHandler = cancelHandler;
	}

	public void SetTotal(int total)
	{
		Total = total;
	}

	public void SetTitle(string title)
	{
		Title = title;
	}

	public virtual void SetMessages(List<ProgressViewModel> messages, bool needClear)
	{
		if (needClear)
		{
			MessageSources.Clear();
		}
		if (messages != null && messages.Count > 0)
		{
			MessageSources.Add(new ObservableCollection<ProgressViewModel>(messages));
		}
	}

	public virtual void SetContents(List<ProgressViewModel> contents)
	{
		Contents.Clear();
		if (contents != null && contents.Count > 0)
		{
			contents.ForEach(delegate(ProgressViewModel n)
			{
				Contents.Add(n);
			});
		}
	}

	public virtual void NotifyIncrementCompletedCount(int completed)
	{
		Completed += completed;
		TotalCompleted += completed;
		Notify();
	}

	public virtual void NotifyRate(ProgressRateInfo rate)
	{
		CurrentResourceRate = "(" + rate.TotalPace + "/" + rate.TotalLength + ")";
		Notify();
	}

	public virtual void NotifyProgress(int currentProgress)
	{
		Completed = currentProgress;
		Notify();
	}

	public void Complete()
	{
		if (!IsCompleted)
		{
			IsCompleted = true;
			Completed = Total;
			Percent = 1.0;
		}
	}

	public virtual void UpdateContensCompleted(double completed)
	{
		if (Contents != null && Contents.Count == 3)
		{
			Contents[1].Message = completed.ToString();
		}
	}

	public void SetButtonContent(string content)
	{
		ButtonContent = content;
	}

	public override void Reset()
	{
		Total = 0;
		Completed = 0;
		TotalCompleted = 0;
		IsCompleted = false;
		Percent = 0.0;
		Failed = 0;
		WorkStatus = WorkStatus.Normal;
		ButtonContent = "K0208";
		CloseWindowCallback = null;
	}

	public virtual void ResetProgress()
	{
		IsCompleted = false;
		Completed = 0;
		Percent = 0.0;
	}

	protected virtual void ClickCommandHandler(object pramater)
	{
		if (WorkStatus == WorkStatus.Normal)
		{
			if (MessageBox.ShowMessage("K0208", "K0541", "K0327", "K0208", isCloseBtn: true) == false)
			{
				return;
			}
			_CancelHandler.Cancel();
		}
		Window obj = pramater as Window;
		CloseWindowCallback?.BeginInvoke(null, null, null);
		Reset();
		obj.Close();
	}

	protected virtual void Notify()
	{
		Percent = (double)Completed / (double)Total;
		if (Completed >= Total && !IsCompleted)
		{
			Complete();
		}
		UpdateContensCompleted(Completed);
	}
}
