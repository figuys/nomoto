using System.Threading;

namespace lenovo.mbg.service.framework.updateversion;

public class UpdateVersionAutoPush
{
	private Timer AutoPushTimer;

	private const long interval = 300000L;

	private long stopInterlocked;

	private long runningInterlocked;

	private object lockerTimer = new object();

	private UpdateWoker m_worker;

	private bool IsRunning => Interlocked.Read(ref runningInterlocked) != 0;

	public bool IsStop => Interlocked.Read(ref stopInterlocked) != 0;

	public UpdateVersionAutoPush(UpdateWoker worker)
	{
		m_worker = worker;
	}

	public void Start()
	{
		Interlocked.Exchange(ref stopInterlocked, 0L);
		if (AutoPushTimer != null)
		{
			return;
		}
		lock (lockerTimer)
		{
			if (AutoPushTimer == null)
			{
				AutoPushTimer = new Timer(FireAutoPush, null, 0L, 300000L);
			}
		}
	}

	public void Stop()
	{
		Interlocked.Exchange(ref stopInterlocked, 1L);
	}

	private void FireAutoPush(object data)
	{
		if (IsRunning)
		{
			return;
		}
		Interlocked.Exchange(ref runningInterlocked, 1L);
		try
		{
			if (IsStop)
			{
				DisposeTimer();
			}
			else if (m_worker != null)
			{
				m_worker.CheckVersion(isAutoMode: true);
			}
		}
		finally
		{
			Interlocked.Exchange(ref runningInterlocked, 0L);
		}
	}

	private void DisposeTimer()
	{
		lock (lockerTimer)
		{
			AutoPushTimer.Dispose();
			AutoPushTimer = null;
		}
	}
}
