using System;
using System.Collections.Concurrent;
using LmsaWindowsService.Contracts;

namespace LmsaWindowsService;

public class PipeWorkerFactory
{
	private static ConcurrentDictionary<string, object> _cached = new ConcurrentDictionary<string, object>();

	public static TResultType CreateInstance<TResultType>(string fullname) where TResultType : class, IPipeMessageWorker
	{
		if (_cached.ContainsKey(fullname))
		{
			return (TResultType)_cached[fullname];
		}
		return Create<TResultType>(fullname);
	}

	protected static TResultType Create<TResultType>(string fullname) where TResultType : class, IPipeMessageWorker
	{
		Type type = typeof(PipeWorkerFactory).Assembly.GetType(fullname);
		if (type == null)
		{
			throw new NotImplementedException($"Don't find the implementation calss for {fullname}");
		}
		TResultType val = (TResultType)Activator.CreateInstance(type);
		_cached.TryAdd(fullname, val);
		return val;
	}
}
