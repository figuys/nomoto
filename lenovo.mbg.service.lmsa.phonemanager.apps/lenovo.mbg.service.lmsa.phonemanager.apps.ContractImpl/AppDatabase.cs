using System.IO;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.phonemanager.apps.Contract;

namespace lenovo.mbg.service.lmsa.phonemanager.apps.ContractImpl;

public class AppDatabase : IAppDatabase
{
	public static string SavePath => Path.Combine(Configurations.AppIconCacheDir, "iconmappingtable.json");

	public T Get<T>(string path) where T : class
	{
		return JsonHelper.DeserializeJson2ObjcetFromFile<T>(path);
	}

	public bool Save(string path, object data)
	{
		return JsonHelper.SerializeObject2File(path, data);
	}
}
