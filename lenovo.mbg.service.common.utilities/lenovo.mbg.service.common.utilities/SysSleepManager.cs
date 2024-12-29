using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace lenovo.mbg.service.common.utilities;

public class SysSleepManager
{
	[Flags]
	public enum EXECUTION_STATE : uint
	{
		ES_AWAYMODE_REQUIRED = 0x40u,
		ES_CONTINUOUS = 0x80000000u,
		ES_DISPLAY_REQUIRED = 2u,
		ES_SYSTEM_REQUIRED = 1u
	}

	private static AutoResetEvent _event = new AutoResetEvent(initialState: false);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	protected static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

	public static void PreventSleep()
	{
		Task.Factory.StartNew(delegate
		{
			SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
			_event.WaitOne();
		}, TaskCreationOptions.LongRunning);
	}

	public static void ResetSleep()
	{
		_event.Set();
	}
}
