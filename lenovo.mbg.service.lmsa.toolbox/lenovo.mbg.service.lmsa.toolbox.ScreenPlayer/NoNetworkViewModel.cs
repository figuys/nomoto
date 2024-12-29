using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class NoNetworkViewModel : NotifyBase
{
	public ReplayCommand TutorialCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public bool IsOk { get; private set; }

	public NoNetworkViewModel(Window wnd)
	{
		NoNetworkViewModel noNetworkViewModel = this;
		TutorialCommand = new ReplayCommand(delegate
		{
			noNetworkViewModel.IsOk = true;
			wnd.Close();
		});
		CloseCommand = new ReplayCommand(delegate
		{
			noNetworkViewModel.IsOk = false;
			wnd.Close();
		});
	}
}
