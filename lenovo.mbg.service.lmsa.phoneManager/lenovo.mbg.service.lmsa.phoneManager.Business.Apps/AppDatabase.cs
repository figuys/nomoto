using System.IO;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.phoneManager.Business.Apps;

public class AppDatabase
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
