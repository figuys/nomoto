using System;
using System.Collections.Concurrent;

namespace lenovo.mbg.service.common.utilities;

public sealed class ContainerManager : IDisposable
{
	private ConcurrentDictionary<string, object> Container;

	private static readonly ContainerManager m_Instance = new ContainerManager();

	public static ContainerManager Instance => m_Instance;

	public void RegisterService(string key, object value)
	{
		if (!Container.ContainsKey(key))
		{
			Container.TryAdd(key, value);
		}
	}

	public T GetService<T>(string key)
	{
		T result = default(T);
		try
		{
			result = (T)Container[key];
			return result;
		}
		catch (Exception)
		{
		}
		return result;
	}

	private ContainerManager()
	{
		Container = new ConcurrentDictionary<string, object>();
	}

	public void Dispose()
	{
		if (Container != null)
		{
			Container.Clear();
			Container = null;
		}
	}
}
