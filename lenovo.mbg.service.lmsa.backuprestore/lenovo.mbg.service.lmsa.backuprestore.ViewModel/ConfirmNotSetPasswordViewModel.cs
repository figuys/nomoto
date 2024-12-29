using System.Windows;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class ConfirmNotSetPasswordViewModel : OKCancelViewModel
{
	private ReplayCommand closeButtonClickCommand;

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

	public ConfirmNotSetPasswordViewModel()
	{
		CloseButtonClickCommand = new ReplayCommand(CloseButtonClickCommandHandler);
	}

	private void CloseButtonClickCommandHandler(object args)
	{
		base.CancelButtonClickCommandHandler(args);
		(args as Window)?.Close();
	}
}
