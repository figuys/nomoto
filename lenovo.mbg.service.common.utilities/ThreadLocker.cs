using System;
using System.Threading;

namespace lenovo.mbg.service.common.utilities;

public class ThreadLocker : IDisposable
{
	private object locker = new object();

	private volatile bool disposed;

	private Func<dynamic> getter = () => (dynamic)null;

	private Action<dynamic> setter = delegate
	{
	};

	public dynamic Data
	{
		get
		{
			return getter();
		}
		set
		{
			((Action<object>)setter)(value);
		}
	}

	public ThreadLocker(Func<dynamic> getter)
		: this(null, getter, null)
	{
	}

	public ThreadLocker(Action<dynamic> setter)
		: this(null, null, setter)
	{
	}

	public ThreadLocker(object locker, Func<dynamic> getter, Action<dynamic> setter)
	{
		if (locker != null)
		{
			this.locker = locker;
		}
		Monitor.Enter(this.locker);
		this.getter = getter;
		this.setter = setter;
	}

	public void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			Monitor.Exit(locker);
		}
		disposed = true;
	}
}
