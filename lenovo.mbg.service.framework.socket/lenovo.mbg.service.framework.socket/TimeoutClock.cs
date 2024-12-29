using System;
using System.Diagnostics;
using System.Threading;

namespace lenovo.mbg.service.framework.socket;

public class TimeoutClock : IDisposable
{
	private Action _timeoutHandler;

	private readonly long TIMEOUT_MILLISECONDS;

	private Stopwatch _stopwatch;

	private volatile bool _isRunning;

	private volatile bool _isDisposed;

	private Timer _heartbeatSender;

	public TimeoutClock(long timeoutMillisencods, Action timeoutHandler)
	{
		_stopwatch = new Stopwatch();
		TIMEOUT_MILLISECONDS = timeoutMillisencods;
		_timeoutHandler = timeoutHandler;
	}

	public void StartClock()
	{
		if (!_isDisposed && !_isRunning)
		{
			_isRunning = true;
			_stopwatch.Start();
			if (_heartbeatSender == null)
			{
				_heartbeatSender = new Timer(TimerCallback, null, 0, 1000);
			}
		}
	}

	private void TimerCallback(object state)
	{
		if (_isRunning && !_isDisposed && _stopwatch != null && _stopwatch.ElapsedMilliseconds > TIMEOUT_MILLISECONDS)
		{
			_stopwatch.Stop();
			_timeoutHandler();
		}
	}

	public void StopClock()
	{
		_isRunning = false;
		_ = _isDisposed;
	}

	public void ResetStart()
	{
		if (!_isDisposed)
		{
			_stopwatch?.Restart();
		}
	}

	public void Dispose()
	{
		_isDisposed = true;
		_stopwatch?.Stop();
		_stopwatch = null;
	}
}
