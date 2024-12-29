using System;
using System.Collections.Concurrent;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.services;

public class RuntimeContext
{
	private DeviceEx device;

	private ConcurrentDictionary<Type, object> objContainer = new ConcurrentDictionary<Type, object>();

	private RuntimeContext(DeviceEx device)
	{
		this.device = device;
	}

	public T GetDevice<T>() where T : DeviceEx
	{
		return device as T;
	}

	public T RegisterOrGetObj<T>()
	{
		T val = Activator.CreateInstance<T>();
		Type typeFromHandle = typeof(T);
		if (objContainer.TryAdd(typeFromHandle, val))
		{
			return val;
		}
		return default(T);
	}
}
