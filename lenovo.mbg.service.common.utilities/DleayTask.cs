using System;
using System.Threading;
using System.Threading.Tasks;

namespace lenovo.mbg.service.common.utilities;

public class DleayTask
{
	private long mDelay;

	private DateTime mTimeline;

	private Action<object> mTask;

	private object mArgs;

	private readonly object mTimelineLock = new object();

	private volatile bool mTaskExcuted = true;

	public DleayTask()
	{
		mTimeline = DateTime.Now;
	}

	public void ReplacePrevTaskAndStart(int delay, Action<object> task, object args)
	{
		if (task == null)
		{
			return;
		}
		lock (mTimelineLock)
		{
			mTask = task;
			mArgs = args;
			mDelay = delay;
			mTimeline = DateTime.Now;
			if (!mTaskExcuted)
			{
				return;
			}
			mTaskExcuted = false;
			Task.Run(delegate
			{
				try
				{
					SpinWait spinWait = default(SpinWait);
					Action<object> action = null;
					object obj = null;
					while (true)
					{
						lock (mTimelineLock)
						{
							action = mTask;
							obj = mArgs;
							if ((DateTime.Now - mTimeline).TotalMilliseconds < (double)mDelay)
							{
								break;
							}
						}
						spinWait.SpinOnce();
					}
					action(obj);
				}
				finally
				{
					mTaskExcuted = true;
				}
			});
		}
	}
}
