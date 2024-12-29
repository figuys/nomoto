using System;
using System.Windows;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.ViewModelV6;

public class ConfirmAppPermissionViewModel : ViewModelBase
{
	private object _CurrentView;

	private bool confirmButtonEnabled = true;

	private Visibility checkButtonVisibility;

	public ReplayCommand CloseCommand { get; }

	public ReplayCommand ConfirmCommand { get; }

	public ReplayCommand LenovoPrivacyClickCommand { get; }

	public Action<object> CloseCallback { get; set; }

	public Action<object> ConfirmCallback { get; set; }

	public object CurrentView
	{
		get
		{
			return _CurrentView;
		}
		set
		{
			_CurrentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public bool ConfirmButtonEnabled
	{
		get
		{
			return confirmButtonEnabled;
		}
		set
		{
			confirmButtonEnabled = value;
			OnPropertyChanged("ConfirmButtonEnabled");
		}
	}

	public Visibility CheckButtonVisibility
	{
		get
		{
			return checkButtonVisibility;
		}
		set
		{
			checkButtonVisibility = value;
			OnPropertyChanged("CheckButtonVisibility");
		}
	}

	public ConfirmAppPermissionViewModel()
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		ConfirmCommand = new ReplayCommand(ConfirmCommandHandler);
		LenovoPrivacyClickCommand = new ReplayCommand(LenovoPrivacyClickCommandHandler);
	}

	private void CloseCommandHandler(object data)
	{
		CloseCallback?.Invoke(data);
	}

	private void ConfirmCommandHandler(object data)
	{
		ConfirmCallback?.Invoke(data);
	}

	private void LenovoPrivacyClickCommandHandler(object args)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}
}
