using lenovo.mbg.service.framework.services;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.hardwaretest.ViewModel;

public class StartViewModel : ViewModelBase
{
	public ReplayCommand GotoWifiCommand { get; }

	public MainFrameViewModel MainFrame => Context.MainFrame;

	public StartViewModel()
	{
		GotoWifiCommand = new ReplayCommand(GotoWifiCommandHandler);
	}

	private void GotoWifiCommandHandler(object data)
	{
		IUserMsgControl userUi = new WifiConnectHelpWindowV6(isHwPop: true, string.Empty, WifiTutorialsType.HWTEST)
		{
			DataContext = new WifiConnectHelpWindowModelV6(WifiTutorialsType.HWTEST)
		};
		Context.MessageBox.ShowMessage(userUi);
	}
}
