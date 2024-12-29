using System;
using System.Windows;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.ViewModelV6;

public class ConfirmAppPermissionIsReadyViewModelV6 : ViewModelBase
{
	public Action<bool?> ManualColseNotifyEvent;

	private Visibility _headerVisibility;

	private Visibility closeButtonVisibility;

	private Visibility checkButtonVisibility;

	private ReplayCommand confirmCommand;

	private ReplayCommand closeButtonClickCommand;

	private bool confirmButtonEnabled = true;

	private FrameworkElement viewContent;

	private ReplayCommand lenovoPrivacyClickCommand;

	private Visibility lenovoPrivacyVisibility = Visibility.Collapsed;

	public Visibility HeaderVisibility
	{
		get
		{
			return _headerVisibility;
		}
		set
		{
			if (_headerVisibility != value)
			{
				_headerVisibility = value;
				OnPropertyChanged("HeaderVisibility");
			}
		}
	}

	public Visibility CloseButtonVisibility
	{
		get
		{
			return closeButtonVisibility;
		}
		set
		{
			if (closeButtonVisibility != value)
			{
				closeButtonVisibility = value;
				OnPropertyChanged("CloseButtonVisibility");
			}
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
			if (checkButtonVisibility != value)
			{
				checkButtonVisibility = value;
				OnPropertyChanged("CheckButtonVisibility");
			}
		}
	}

	public ReplayCommand ConfirmCommand
	{
		get
		{
			return confirmCommand;
		}
		set
		{
			if (confirmCommand != value)
			{
				confirmCommand = value;
				OnPropertyChanged("ConfirmCommand");
			}
		}
	}

	public ReplayCommand CloseButtonClickCommand
	{
		get
		{
			return closeButtonClickCommand;
		}
		set
		{
			if (closeButtonClickCommand != value)
			{
				closeButtonClickCommand = value;
				OnPropertyChanged("CloseButtonClickCommand");
			}
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
			if (confirmButtonEnabled != value)
			{
				confirmButtonEnabled = value;
				OnPropertyChanged("ConfirmButtonEnabled");
			}
		}
	}

	public FrameworkElement ViewContent
	{
		get
		{
			return viewContent;
		}
		set
		{
			if (viewContent != value)
			{
				viewContent = value;
				OnPropertyChanged("ViewContent");
			}
		}
	}

	public ReplayCommand LenovoPrivacyClickCommand
	{
		get
		{
			return lenovoPrivacyClickCommand;
		}
		set
		{
			if (lenovoPrivacyClickCommand != value)
			{
				lenovoPrivacyClickCommand = value;
				OnPropertyChanged("LenovoPrivacyClickCommand");
			}
		}
	}

	public Visibility LenovoPrivacyVisibility
	{
		get
		{
			return lenovoPrivacyVisibility;
		}
		set
		{
			if (lenovoPrivacyVisibility != value)
			{
				lenovoPrivacyVisibility = value;
				OnPropertyChanged("LenovoPrivacyVisibility");
			}
		}
	}

	public ConfirmAppPermissionIsReadyViewModelV6()
	{
		CloseButtonClickCommand = new ReplayCommand(CloseButtonClickCommandHandler);
		LenovoPrivacyClickCommand = new ReplayCommand(LenovoPrivacyClickCommandHandler);
	}

	private void CloseButtonClickCommandHandler(object args)
	{
		ManualColseNotifyEvent?.Invoke(null);
		(args as Window)?.Close();
	}

	private void LenovoPrivacyClickCommandHandler(object args)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}
}
