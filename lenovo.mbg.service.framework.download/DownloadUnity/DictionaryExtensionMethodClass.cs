using System.Collections.Concurrent;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public static class DictionaryExtensionMethodClass
{
	public static bool TryAddEx<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
	{
		if (!dict.ContainsKey(key))
		{
			return dict.TryAdd(key, value);
		}
		return false;
	}
}
