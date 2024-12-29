using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.Feedback.Business;
using lenovo.mbg.service.lmsa.Feedback.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackListViewModelV6 : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private string mComments;

	private ObservableCollection<FeedbackItemViewModel> feedbackItems;

	private string winTitle;

	private ReplayCommand replyCommand;

	private bool logFileIsChecked;

	private bool submitButtonIsEnabled = false;

	public string Comments
	{
		get
		{
			return mComments;
		}
		set
		{
			if (!(mComments == value))
			{
				mComments = value;
				OnPropertyChanged("Comments");
			}
		}
	}

	public FeedBackNodeModel FeedBack { get; set; }

	public ObservableCollection<FeedbackItemViewModel> FeedbackItems
	{
		get
		{
			return feedbackItems;
		}
		set
		{
			if (feedbackItems != value)
			{
				feedbackItems = value;
				OnPropertyChanged("FeedbackItems");
			}
		}
	}

	public string WinTitle
	{
		get
		{
			return winTitle;
		}
		set
		{
			if (!(winTitle == value))
			{
				winTitle = value;
				OnPropertyChanged("WinTitle");
			}
		}
	}

	public ReplayCommand ReplyCommand
	{
		get
		{
			return replyCommand;
		}
		set
		{
			if (replyCommand != value)
			{
				replyCommand = value;
				OnPropertyChanged("ReplyCommand");
			}
		}
	}

	public bool LogFileIsChekced
	{
		get
		{
			return logFileIsChecked;
		}
		set
		{
			if (logFileIsChecked != value)
			{
				logFileIsChecked = value;
				OnPropertyChanged("LogFileIsChekced");
			}
		}
	}

	public bool SubmitButtonIsEnabled
	{
		get
		{
			return submitButtonIsEnabled;
		}
		set
		{
			if (submitButtonIsEnabled != value)
			{
				submitButtonIsEnabled = value;
				OnPropertyChanged("SubmitButtonIsEnabled");
			}
		}
	}

	public FeedbackListViewModelV6()
	{
		ReplyCommand = new ReplayCommand(ReplyCommandHandler);
	}

	private async void ReplyCommandHandler(object args)
	{
		Window win = args as Window;
		if (FeedBack == null || string.IsNullOrWhiteSpace(Comments))
		{
			return;
		}
		long? latestFeedbackId = null;
		FeedBackNodeModel current = FeedBack;
		while (current != null)
		{
			if ("Q".Equals(current.Type))
			{
				latestFeedbackId = current.Id;
			}
			if (current.Children != null)
			{
				current = current.Children.FirstOrDefault((FeedBackNodeModel m) => m.Children != null && m.Children.Count > 0);
			}
		}
		FeedbackSubmitModel model = new FeedbackSubmitModel
		{
			FeedbackContent = Comments,
			FeedbackId = latestFeedbackId,
			IsReplay = (FeedBack != null)
		};
		bool isUploadlog = (logFileIsChecked ? LogFileIsChekced : ApplcationClass.ApplcationStartWindow.ShowMessage("K0829", MessageBoxButton.YesNo).Value);
		SubmitButtonIsEnabled = false;
		FeedBackBLL bll = new FeedBackBLL();
		List<string> files = (isUploadlog ? bll.GetLogFileCopy() : null);
		bool isSuccess = await bll.SubmitFeedbackAsync(model, files);
		if (files != null && files.Count > 0)
		{
			files.ForEach(delegate(string n)
			{
				if (!string.IsNullOrEmpty(n))
				{
					GlobalFun.TryDeleteFile(n);
				}
			});
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			win?.Close();
			if (isSuccess)
			{
				ApplcationClass.ApplcationStartWindow.ShowMessage("K0733");
			}
			else
			{
				ApplcationClass.ApplcationStartWindow.ShowMessage("K0734");
			}
		});
	}
}
