using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.common.utilities;

public class TimedCache<Tkey, TValue>
{
	private SortedList<Tkey, TValue> _Cache = new SortedList<Tkey, TValue>();

	private SortedList<Tkey, DateTime> _Expirations = new SortedList<Tkey, DateTime>();

	public TValue this[Tkey key]
	{
		get
		{
			lock (this)
			{
				Refresh();
				if (_Cache.ContainsKey(key))
				{
					return _Cache[key];
				}
				return default(TValue);
			}
		}
	}

	public int Count
	{
		get
		{
			lock (this)
			{
				Refresh();
				return _Cache.Count;
			}
		}
	}

	public IList<Tkey> Keys
	{
		get
		{
			lock (this)
			{
				Refresh();
				return _Cache.Keys;
			}
		}
	}

	public IList<TValue> Values
	{
		get
		{
			lock (this)
			{
				Refresh();
				return _Cache.Values;
			}
		}
	}

	public void Add(Tkey key, TValue value)
	{
		Add(key, value, TimeSpan.FromDays(30.0));
	}

	public void Add(Tkey key, TValue value, TimeSpan expiration)
	{
		lock (this)
		{
			Refresh();
			_Expirations[key] = DateTime.Now.Add(expiration);
			_Cache[key] = value;
		}
	}

	public void Remove(Tkey key)
	{
		lock (this)
		{
			if (_Expirations.ContainsKey(key))
			{
				_Expirations.Remove(key);
			}
			if (_Cache.ContainsKey(key))
			{
				_Cache.Remove(key);
			}
		}
	}

	public IEnumerator<KeyValuePair<Tkey, TValue>> GetEnumerator()
	{
		lock (this)
		{
			Refresh();
			return _Cache.GetEnumerator();
		}
	}

	public bool ContainsKey(Tkey key)
	{
		lock (this)
		{
			Refresh();
			return _Cache.ContainsKey(key);
		}
	}

	public void Clear()
	{
		lock (this)
		{
			_Cache.Clear();
			_Expirations.Clear();
		}
	}

	private void Refresh()
	{
		List<Tkey> list = new List<Tkey>();
		foreach (Tkey key in _Expirations.Keys)
		{
			DateTime value = _Expirations[key];
			if (DateTime.Now.CompareTo(value) > 0)
			{
				list.Add(key);
			}
		}
		foreach (Tkey item in list)
		{
			_Cache.Remove(item);
			_Expirations.Remove(item);
		}
	}
}
