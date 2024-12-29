namespace lenovo.mbg.service.framework.services.Adb;

public interface IAdbService
{
	string Execute(string cmd);

	void StartServer();

	void KillServer();
}
