using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.ViewModels;

namespace lenovo.mbg.service.lmsa;

public class HostNavgationService : IHostNavigation
{
	public string CurrentPluginID { get; set; }

	public void SwitchTo(string pluginID)
	{
		MainWindowViewModel.SingleInstance.GotoPluginById(pluginID);
	}

	public void SwitchTo(string pluginID, object data)
	{
		MainWindowViewModel.SingleInstance.GotoPluginById(pluginID, data);
	}

	public void SwithStyle(string styleId)
	{
	}
}
