namespace lenovo.mbg.service.lmsa.phonemanager.apps.Contract;

public interface IAppDatabase
{
	bool Save(string path, object data);

	T Get<T>(string path) where T : class;
}
