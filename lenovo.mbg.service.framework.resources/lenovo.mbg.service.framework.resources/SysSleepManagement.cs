using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace lenovo.mbg.service.framework.resources;

public class SysSleepManagement
{
	[Flags]
	private enum ExecutionFlag : uint
	{
		System = 1u,
		Display = 2u,
		Continus = 0x80000000u
	}

	[DllImport("kernel32.dll")]
	private static extern uint SetThreadExecutionState(ExecutionFlag flags);

	[DllImport("kernel32.dll")]
	public static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, bool fResume);

	public static void SetWaitForWakeUpTime(int minutes)
	{
		long pDueTime = DateTime.Now.AddMinutes(minutes).ToFileTime();
		using SafeWaitHandle safeWaitHandle = CreateWaitableTimer(IntPtr.Zero, bManualReset: true, "LenovoWaitabletimer");
		if (SetWaitableTimer(safeWaitHandle, ref pDueTime, 0, IntPtr.Zero, IntPtr.Zero, fResume: true))
		{
			using (EventWaitHandle eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset))
			{
				eventWaitHandle.SafeWaitHandle = safeWaitHandle;
				eventWaitHandle.WaitOne();
				return;
			}
		}
		throw new Win32Exception(Marshal.GetLastWin32Error());
	}

	public static void PreventSleep(bool includeDisplay)
	{
		if (includeDisplay)
		{
			SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Display | ExecutionFlag.Continus);
		}
		else
		{
			SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Continus);
		}
	}

	public static void RestoreSleep()
	{
		SetThreadExecutionState(ExecutionFlag.Continus);
	}

	public static void ResetSleepTimer(bool includeDisplay)
	{
		if (includeDisplay)
		{
			SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Display);
		}
		else
		{
			SetThreadExecutionState(ExecutionFlag.System);
		}
	}
}
