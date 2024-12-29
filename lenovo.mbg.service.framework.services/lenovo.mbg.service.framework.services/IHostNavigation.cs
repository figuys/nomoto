namespace lenovo.mbg.service.framework.services;

public interface IHostNavigation
{
	string CurrentPluginID { get; set; }

	void SwitchTo(string pluginID);

	void SwitchTo(string pluginID, object data);
}
