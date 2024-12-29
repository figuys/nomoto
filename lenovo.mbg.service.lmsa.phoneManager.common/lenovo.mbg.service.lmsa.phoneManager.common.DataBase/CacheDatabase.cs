using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.phoneManager.common.DataBase;

public class CacheDatabase : ICacheDatabase
{
	protected static readonly Dictionary<CacheDataType, string> _SavePathDictionary = new Dictionary<CacheDataType, string>
	{
		{
			CacheDataType.APP,
			Path.Combine(Configurations.AppIconCacheDir, "iconmappingtable.json")
		},
		{
			CacheDataType.MUSIC,
			Path.Combine(Configurations.MusicCacheDir, "music.json")
		}
	};

	public string SavePath { get; private set; }

	public CacheDatabase(CacheDataType cacheType)
	{
		SavePath = _SavePathDictionary[cacheType];
	}

	public T Get<T>() where T : class
	{
		return JsonHelper.DeserializeJson2ObjcetFromFile<T>(SavePath);
	}

	public T Get<T>(string path) where T : class
	{
		return JsonHelper.DeserializeJson2ObjcetFromFile<T>(path);
	}

	public bool Save(object data)
	{
		return JsonHelper.SerializeObject2File(SavePath, data);
	}

	public bool Save(string path, object data)
	{
		return JsonHelper.SerializeObject2File(path, data);
	}
}
