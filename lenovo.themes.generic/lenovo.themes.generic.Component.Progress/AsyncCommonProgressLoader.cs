using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.Exceptions;

namespace lenovo.themes.generic.Component.Progress;

public class AsyncCommonProgressLoader
{
	private Dispatcher mCurrentDispatcher;

	private HostMaskLayerWrapper mHostMaskLayerWrapper;

	public CommonProgressWindowViewModel ViewModel { get; private set; }

	public AsyncCommonProgressLoader(Dispatcher dispatcher, HostMaskLayerWrapper hostMaskLayerWrapper)
	{
		mCurrentDispatcher = dispatcher;
		mHostMaskLayerWrapper = hostMaskLayerWrapper;
	}

	public void Progress(IMessageBox messageBox, Action<IAsyncTaskContext, CommonProgressWindowViewModel> task)
	{
		Progress(messageBox, task, null);
	}

	public void Progress(IMessageBox messageBox, Action<IAsyncTaskContext, CommonProgressWindowViewModel> task, Action<IAsyncTaskResult, WorkStatus> taskExecutedCallback)
	{
		Progress(messageBox, task, null, taskExecutedCallback, null);
	}

	public void Progress(IMessageBox messageBox, Action<IAsyncTaskContext, CommonProgressWindowViewModel> task, Action<IAsyncTaskResult, WorkStatus> taskExecutedCallback, Action<object> windowClosedCallback)
	{
		Progress(messageBox, task, null, taskExecutedCallback, windowClosedCallback);
	}

	public void Progress(IMessageBox messageBox, Action<IAsyncTaskContext, CommonProgressWindowViewModel> task, Action<CommonProgressWindowViewModel, NotifyTypes, object> taskExecutingCallback, Action<IAsyncTaskResult, WorkStatus> taskExecutedCallback, Action<object> windowClosedCallback)
	{
		if (taskExecutingCallback == null)
		{
			taskExecutingCallback = ExecutingCallback;
		}
		ViewModel = new CommonProgressWindowViewModel(messageBox, mCurrentDispatcher, mHostMaskLayerWrapper, windowClosedCallback);
		IAsyncTaskCancelHander cancelHandler = AsyncTaskRunner.BeginInvok(ConvertTask(task), ConvertExecutedCallback(taskExecutedCallback), CovertExecutingCallback(taskExecutingCallback));
		ViewModel.SetCancelHandler(cancelHandler);
		mCurrentDispatcher.Invoke(delegate
		{
			Window window = new CommonProgressWindowV6
			{
				DataContext = ViewModel
			};
			mHostMaskLayerWrapper.New(window, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				window.ShowDialog();
			});
		});
	}

	public IAsyncTaskCancelHander Progress(Action<IAsyncTaskContext> task, Action<NotifyTypes, object> taskExecutingCallback, Action<IAsyncTaskResult, WorkStatus> taskExecutedCallback)
	{
		return AsyncTaskRunner.BeginInvok(task, ConvertExecutedCallback(taskExecutedCallback), taskExecutingCallback);
	}

	protected Action<IAsyncTaskContext> ConvertTask(Action<IAsyncTaskContext, CommonProgressWindowViewModel> task)
	{
		return delegate(IAsyncTaskContext context)
		{
			task(context, ViewModel);
		};
	}

	protected Action<IAsyncTaskResult> ConvertExecutedCallback(Action<IAsyncTaskResult, WorkStatus> callback)
	{
		return delegate(IAsyncTaskResult result)
		{
			WorkStatus status = WorkStatus.Normal;
			if (result.IsCancelCommandRequested)
			{
				status = WorkStatus.Cancel;
			}
			else if (result.Exception == null)
			{
				status = WorkStatus.Success;
			}
			else if (result.Exception != null && !(result.Exception is CacnelException))
			{
				status = WorkStatus.Failed;
			}
			mCurrentDispatcher.Invoke(delegate
			{
				if (ViewModel != null)
				{
					ViewModel.WorkStatus = status;
				}
				callback?.Invoke(result, status);
			});
		};
	}

	protected Action<NotifyTypes, object> CovertExecutingCallback(Action<CommonProgressWindowViewModel, NotifyTypes, object> callback)
	{
		return delegate(NotifyTypes type, object pramater)
		{
			callback?.Invoke(ViewModel, type, pramater);
		};
	}

	protected void ExecutingCallback(CommonProgressWindowViewModel viewModel, NotifyTypes notifyType, object pramater)
	{
		mCurrentDispatcher.Invoke(delegate
		{
			bool flag = true;
			List<object> list = null;
			switch (notifyType)
			{
			case NotifyTypes.INITILIZE:
				list = pramater as List<object>;
				Initilize(viewModel, list);
				break;
			case NotifyTypes.PERCENT:
			{
				int completed = (int)pramater;
				viewModel.NotifyIncrementCompletedCount(completed);
				break;
			}
			case NotifyTypes.PERCENTEX:
			{
				ProgressRateInfo rate = (ProgressRateInfo)pramater;
				viewModel.NotifyRate(rate);
				break;
			}
			case NotifyTypes.PROGRESSINFO:
				list = pramater as List<object>;
				viewModel.Initilize((int)list[1], ConvertContents(list));
				viewModel.ResetProgress();
				break;
			case NotifyTypes.FAILEDCOUNT:
			{
				int num = (int)pramater;
				viewModel.Failed += num;
				break;
			}
			case NotifyTypes.NOT_ENOUGH_SPACE:
				flag = (bool)pramater;
				viewModel.SetMessages(ConvertMessage("K0623", "#ff0000"), flag);
				break;
			case NotifyTypes.FAILD_FILE_NOT_EXISTS:
				flag = (bool)pramater;
				viewModel.SetMessages(ConvertMessage("The picture doesn't exist in your phone any more", "#ff0000"), flag);
				break;
			case NotifyTypes.ERROR_MESSAGE:
				list = pramater as List<object>;
				viewModel.SetMessages(ConvertMessage(list[0].ToString(), "#ff0000"), (bool)list[1]);
				break;
			case NotifyTypes.INFO_MESSAGE:
				list = pramater as List<object>;
				viewModel.SetMessages(ConvertMessage(list[0].ToString(), "#494949"), (bool)list[1]);
				break;
			case NotifyTypes.CANCEL:
				ViewModel.WorkStatus = WorkStatus.Cancel;
				break;
			case NotifyTypes.SUCCESS:
				ViewModel.WorkStatus = WorkStatus.Success;
				SetMessage(ViewModel, pramater);
				break;
			case NotifyTypes.FAILED:
				ViewModel.WorkStatus = WorkStatus.Failed;
				SetMessage(ViewModel, pramater);
				break;
			}
		});
	}

	protected List<ProgressViewModel> ConvertContents(List<object> infos)
	{
		return new List<ProgressViewModel>
		{
			new ProgressViewModel
			{
				Message = $"{LangTranslation.Translate(infos[0].ToString())} ("
			},
			new ProgressViewModel
			{
				Message = "0",
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"))
			},
			new ProgressViewModel
			{
				Message = $"/{infos[1].ToString()})"
			}
		};
	}

	protected List<ProgressViewModel> ConvertMessage(string message, string foreground)
	{
		return new List<ProgressViewModel>
		{
			new ProgressViewModel
			{
				Message = message,
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foreground))
			}
		};
	}

	private List<ProgressViewModel> Convert2ProgressViewModel(List<ProgressPramater> pramater)
	{
		List<ProgressViewModel> result = new List<ProgressViewModel>();
		pramater.ForEach(delegate(ProgressPramater n)
		{
			ProgressViewModel progressViewModel = new ProgressViewModel
			{
				Message = n.Message
			};
			if (!string.IsNullOrEmpty(n.Foreground))
			{
				progressViewModel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(n.Foreground));
			}
			if (n.FontSize > 0.0)
			{
				progressViewModel.FontSize = n.FontSize;
			}
			result.Add(progressViewModel);
		});
		return result;
	}

	private void Initilize(CommonProgressWindowViewModel viewModel, List<object> pramater)
	{
		if (pramater[0] != null)
		{
			List<object> list = pramater[0] as List<object>;
			viewModel.Initilize((int)list[1], ConvertContents(list));
		}
		if (pramater[1] != null)
		{
			List<ProgressPramater> pramater2 = pramater[1] as List<ProgressPramater>;
			viewModel.SetMessages(Convert2ProgressViewModel(pramater2), needClear: true);
		}
	}

	private void SetMessage(CommonProgressWindowViewModel viewModel, object pramater)
	{
		if (pramater != null)
		{
			List<object> list = pramater as List<object>;
			List<ProgressPramater> pramater2 = list[0] as List<ProgressPramater>;
			viewModel.SetMessages(Convert2ProgressViewModel(pramater2), (bool)list[1]);
		}
	}
}
