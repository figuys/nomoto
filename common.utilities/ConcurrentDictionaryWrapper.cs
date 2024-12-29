using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.common.utilities;

public class ConcurrentDictionaryWrapper<TKey, TValue>
{
	private ConcurrentDictionary<TKey, TValue> m_dic;

	public int Count => m_dic.Count;

	public ConcurrentDictionaryWrapper()
	{
		m_dic = new ConcurrentDictionary<TKey, TValue>();
	}

	public void AddOrUpdate(TKey key, TValue value)
	{
		m_dic.AddOrUpdate(key, value, (TKey k, TValue oldValue) => value);
	}

	public TValue Get(TKey key)
	{
		TValue value = default(TValue);
		m_dic.TryGetValue(key, out value);
		return value;
	}

	public TValue Remove(TKey key)
	{
		TValue value = default(TValue);
		while (m_dic.ContainsKey(key))
		{
			if (m_dic.TryRemove(key, out value))
			{
				return value;
			}
		}
		return value;
	}

	public bool ContainsKey(TKey key)
	{
		return m_dic.ContainsKey(key);
	}

	public List<TValue> GetValues()
	{
		return m_dic.Values.ToList();
	}
}
