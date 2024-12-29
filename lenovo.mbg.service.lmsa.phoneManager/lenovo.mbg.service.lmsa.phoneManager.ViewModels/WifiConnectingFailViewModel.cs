using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class WifiConnectingFailViewModel : ConnectingViewModel
{
	private ReplayCommand goToWIFIConnectPanelCommand;

	public ReplayCommand GoToWIFIConnectPanelCommand
	{
		get
		{
			return goToWIFIConnectPanelCommand;
		}
		set
		{
			if (goToWIFIConnectPanelCommand != value)
			{
				goToWIFIConnectPanelCommand = value;
				OnPropertyChanged("GoToWIFIConnectPanelCommand");
			}
		}
	}

	public WifiConnectingFailViewModel()
	{
		GoToWIFIConnectPanelCommand = new ReplayCommand(GoToWIFIConnectPanelCommandHandler);
	}

	private void GoToWIFIConnectPanelCommandHandler(object args)
	{
		LogHelper.LogInstance.Info("Wifi connected failed, wifi connection page will show!");
		HomePageFrameViewModel.Single.Switch(typeof(WifiConnection));
	}
}
