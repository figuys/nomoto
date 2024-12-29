using System.Collections.Concurrent;
using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.phoneManager.common.DataBase;

public class CacheMapping<TKey, TValue> : CacheDatabase
{
	public IDictionary<TKey, TValue> Cache { get; private set; }

	public int Count => Cache.Count;

	public TValue this[TKey key]
	{
		get
		{
			return Get(key);
		}
		set
		{
			Insert(key, value);
		}
	}

	public CacheMapping(CacheDataType cacheType)
		: base(cacheType)
	{
		Cache = new ConcurrentDictionary<TKey, TValue>();
	}

	public CacheMapping(CacheDataType cacheType, IEqualityComparer<TKey> equalityComparer)
		: base(cacheType)
	{
		Cache = new ConcurrentDictionary<TKey, TValue>(equalityComparer);
	}

	public void Insert(TKey key, TValue value)
	{
		Cache[key] = value;
	}

	public TValue Get(TKey key)
	{
		Cache.TryGetValue(key, out var value);
		return value;
	}

	public bool Remove(TKey key)
	{
		return Cache.Remove(key);
	}

	public void Clear()
	{
		Cache.Clear();
	}
}
