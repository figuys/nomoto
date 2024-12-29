namespace lenovo.mbg.service.lmsa.phoneManager.common.DataBase;

public interface ICacheDatabase
{
	string SavePath { get; }

	bool Save(object data);

	bool Save(string path, object data);

	T Get<T>() where T : class;

	T Get<T>(string path) where T : class;
}
