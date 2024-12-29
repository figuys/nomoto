using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.common.Form.FormVerify;
using lenovo.mbg.service.lmsa.common.Form.ViewModel;
using lenovo.mbg.service.lmsa.Feedback.Business;
using lenovo.mbg.service.lmsa.Feedback.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.UserControls.MessageBoxWindow;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackListViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private List<FormItemViewModel> mFormItems = null;

	private FormItemViewModel mComments;

	private ObservableCollection<FeedbackItemViewModel> feedbackItems;

	private string winTitle;

	private ReplayCommand replyCommand;

	private bool logFileIsChecked;

	private bool submitButtonIsEnabled = true;

	private ReplayCommand closeWinCommand;

	public FormItemViewModel Comments
	{
		get
		{
			return mComments;
		}
		set
		{
			if (mComments != value)
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

	public ReplayCommand CloseWinCommand
	{
		get
		{
			return closeWinCommand;
		}
		set
		{
			if (closeWinCommand != value)
			{
				closeWinCommand = value;
				OnPropertyChanged("CloseWinCommand");
			}
		}
	}

	public FeedbackListViewModel()
	{
		ReplyCommand = new ReplayCommand(ReplyCommandHandler);
		CloseWinCommand = new ReplayCommand(CloseWinCommandHandler);
		mFormItems = new List<FormItemViewModel>();
		Comments = new FormItemViewModel(new CanNotEmptyVerify());
		mFormItems.Add(Comments);
	}

	private async void ReplyCommandHandler(object args)
	{
		Window win = args as Window;
		if (FeedBack == null)
		{
			return;
		}
		foreach (FormItemViewModel item in mFormItems)
		{
			item.Wraning = new FormItemVerifyWraningViewModel();
		}
		if (!Verify())
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
			FeedbackContent = Comments.InputValue,
			FeedbackId = latestFeedbackId,
			IsReplay = (FeedBack != null)
		};
		bool isUploadlog;
		if (!logFileIsChecked)
		{
			CommonWindowModel winModel = new CommonWindowModel(2);
			(winModel.ViewModel as OKCancelViewModel).Title = string.Empty;
			LenovoPopupWindow wnd = winModel.CreateWindow("K0829");
			HostProxy.HostMaskLayerWrapper.New(wnd, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd.ShowDialog());
			isUploadlog = winModel.GetViewModel<OKCancelViewModel>().IsOKResult;
		}
		else
		{
			isUploadlog = LogFileIsChekced;
		}
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
		SubmitButtonIsEnabled = true;
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			win?.Close();
			ShowTips(isSuccess ? "K0733" : "K0734");
		});
	}

	private void ShowTips(string msg)
	{
		string title = "K0711";
		string okButtonText = "K0327";
		LenovoPopupWindow win = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, msg, okButtonText, null);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.Show();
		});
	}

	public bool Verify()
	{
		bool flag = true;
		foreach (FormItemViewModel mFormItem in mFormItems)
		{
			flag &= mFormItem.Verify();
		}
		return flag;
	}

	private void CloseWinCommandHandler(object args)
	{
		if (args is Window window)
		{
			window.Close();
		}
	}
}
