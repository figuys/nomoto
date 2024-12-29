using System;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.common.utilities;

public class CustomThread
{
	private static CustomThread _instance;

	public static CustomThread Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			return _instance = new CustomThread();
		}
	}

	public void Run(ThreadStart task)
	{
		Run(task, setSta: false);
	}

	public void Run(ThreadStart task, bool setSta)
	{
		RunThread(task, setSta);
	}

	public void Run<ReturnValue>(Func<ReturnValue> task, Action<ReturnValue> callback)
	{
		Run(task, callback, setSta: false);
	}

	public void Run<ReturnValue>(Func<ReturnValue> task, Action<ReturnValue> callback, bool setSta)
	{
		ThreadStart task2 = delegate
		{
			ReturnValue obj = task();
			callback(obj);
		};
		Run(task2, setSta);
	}

	public ReturnValue RunAndWait<ReturnValue>(Func<ReturnValue> task)
	{
		return RunAndWait(task, setSta: false);
	}

	public ReturnValue RunAndWait<ReturnValue>(Func<ReturnValue> task, bool setSta)
	{
		ReturnValue returned = default(ReturnValue);
		ThreadStart task2 = delegate
		{
			returned = task();
		};
		RunThread(task2, setSta).Join();
		return returned;
	}

	public Thread RunThread(ThreadStart task)
	{
		return RunThread(task, setSta: false);
	}

	public Thread RunThread(ThreadStart task, bool setSta)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				task();
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("Unhandled exception in thread: ", exception);
			}
		});
		thread.IsBackground = true;
		if (setSta)
		{
			thread.SetApartmentState(ApartmentState.STA);
		}
		thread.Start();
		return thread;
	}

	public void Wait(TimeSpan waitTime)
	{
		Wait(waitTime, null);
	}

	public bool Wait(TimeSpan waitTime, Checker<bool> returnIfTrue)
	{
		return Wait(waitTime, returnIfTrue, valueToWaitFor: true);
	}

	public ReturnType Wait<ReturnType>(TimeSpan waitTime, Checker<ReturnType> checker, ReturnType valueToWaitFor)
	{
		DateTime now = DateTime.Now;
		ReturnType result = default(ReturnType);
		while (DateTime.Now.Subtract(now).TotalMilliseconds < waitTime.TotalMilliseconds)
		{
			if (checker != null)
			{
				result = checker();
				if (result.Equals(valueToWaitFor))
				{
					return result;
				}
			}
		}
		return result;
	}

	public Checker<ReturnType> AddDelay<ReturnType>(Checker<ReturnType> checker, ReturnType valueToWaitFor, TimeSpan delay)
	{
		return () => DelayedCheck(checker, valueToWaitFor, delay);
	}

	private ReturnType DelayedCheck<ReturnType>(Checker<ReturnType> checker, ReturnType valueToWaitFor, TimeSpan delay)
	{
		ReturnType result = checker();
		if (!result.Equals(valueToWaitFor))
		{
			Wait(delay);
		}
		return result;
	}
}
