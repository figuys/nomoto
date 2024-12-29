using System.Threading;

namespace lenovo.mbg.service.framework.socket;

public class Sequence : ISequence
{
	private static Sequence _singleInstance;

	private static long _sequence;

	public static Sequence SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new Sequence();
			}
			return _singleInstance;
		}
	}

	private Sequence()
	{
	}

	public long New()
	{
		return Interlocked.Increment(ref _sequence);
	}
}
