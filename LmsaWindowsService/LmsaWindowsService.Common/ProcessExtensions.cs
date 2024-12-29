using System;
using System.Runtime.InteropServices;

namespace LmsaWindowsService.Common;

public static class ProcessExtensions
{
	private enum SW
	{
		SW_HIDE = 0,
		SW_SHOWNORMAL = 1,
		SW_NORMAL = 1,
		SW_SHOWMINIMIZED = 2,
		SW_SHOWMAXIMIZED = 3,
		SW_MAXIMIZE = 3,
		SW_SHOWNOACTIVATE = 4,
		SW_SHOW = 5,
		SW_MINIMIZE = 6,
		SW_SHOWMINNOACTIVE = 7,
		SW_SHOWNA = 8,
		SW_RESTORE = 9,
		SW_SHOWDEFAULT = 10,
		SW_MAX = 10
	}

	private enum WTS_CONNECTSTATE_CLASS
	{
		WTSActive,
		WTSConnected,
		WTSConnectQuery,
		WTSShadow,
		WTSDisconnected,
		WTSIdle,
		WTSListen,
		WTSReset,
		WTSDown,
		WTSInit
	}

	private struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;

		public IntPtr hThread;

		public uint dwProcessId;

		public uint dwThreadId;
	}

	private enum SECURITY_IMPERSONATION_LEVEL
	{
		SecurityAnonymous,
		SecurityIdentification,
		SecurityImpersonation,
		SecurityDelegation
	}

	private struct STARTUPINFO
	{
		public int cb;

		public string lpReserved;

		public string lpDesktop;

		public string lpTitle;

		public uint dwX;

		public uint dwY;

		public uint dwXSize;

		public uint dwYSize;

		public uint dwXCountChars;

		public uint dwYCountChars;

		public uint dwFillAttribute;

		public uint dwFlags;

		public short wShowWindow;

		public short cbReserved2;

		public IntPtr lpReserved2;

		public IntPtr hStdInput;

		public IntPtr hStdOutput;

		public IntPtr hStdError;
	}

	private enum TOKEN_TYPE
	{
		TokenPrimary = 1,
		TokenImpersonation
	}

	private struct WTS_SESSION_INFO
	{
		public readonly uint SessionID;

		[MarshalAs(UnmanagedType.LPStr)]
		public readonly string pWinStationName;

		public readonly WTS_CONNECTSTATE_CLASS State;
	}

	private const int CREATE_UNICODE_ENVIRONMENT = 1024;

	private const int CREATE_NO_WINDOW = 134217728;

	private const int CREATE_NEW_CONSOLE = 16;

	private const uint INVALID_SESSION_ID = uint.MaxValue;

	private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

	[DllImport("advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
	private static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandle, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

	[DllImport("advapi32.dll")]
	private static extern bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess, IntPtr lpThreadAttributes, int TokenType, int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

	[DllImport("userenv.dll", SetLastError = true)]
	private static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

	[DllImport("userenv.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CloseHandle(IntPtr hSnapshot);

	[DllImport("kernel32.dll")]
	private static extern uint WTSGetActiveConsoleSessionId();

	[DllImport("Wtsapi32.dll")]
	private static extern uint WTSQueryUserToken(uint SessionId, ref IntPtr phToken);

	[DllImport("wtsapi32.dll", SetLastError = true)]
	private static extern int WTSEnumerateSessions(IntPtr hServer, int Reserved, int Version, ref IntPtr ppSessionInfo, ref int pCount);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

	private static bool GetSessionUserToken(ref IntPtr phUserToken)
	{
		bool result = false;
		IntPtr phToken = IntPtr.Zero;
		uint num = uint.MaxValue;
		IntPtr ppSessionInfo = IntPtr.Zero;
		int pCount = 0;
		if (WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref ppSessionInfo, ref pCount) != 0)
		{
			int num2 = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
			IntPtr ptr = ppSessionInfo;
			for (int i = 0; i < pCount; i++)
			{
				WTS_SESSION_INFO wTS_SESSION_INFO = (WTS_SESSION_INFO)Marshal.PtrToStructure(ptr, typeof(WTS_SESSION_INFO));
				ptr += num2;
				if (wTS_SESSION_INFO.State == WTS_CONNECTSTATE_CLASS.WTSActive)
				{
					num = wTS_SESSION_INFO.SessionID;
				}
			}
		}
		if (num == uint.MaxValue)
		{
			num = WTSGetActiveConsoleSessionId();
		}
		if (WTSQueryUserToken(num, ref phToken) != 0)
		{
			result = DuplicateTokenEx(phToken, 0u, IntPtr.Zero, 2, 1, ref phUserToken);
			CloseHandle(phToken);
		}
		return result;
	}

	public static bool StartProcessAsCurrentUser(string appPath, string cmdLine = null, string workDir = null, bool visible = true)
	{
		IntPtr phUserToken = IntPtr.Zero;
		STARTUPINFO lpStartupInfo = default(STARTUPINFO);
		PROCESS_INFORMATION lpProcessInformation = default(PROCESS_INFORMATION);
		IntPtr lpEnvironment = IntPtr.Zero;
		lpStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
		try
		{
			if (!GetSessionUserToken(ref phUserToken))
			{
				throw new Exception("StartProcessAsCurrentUser: GetSessionUserToken failed.");
			}
			uint dwCreationFlags = (uint)(0x400 | (visible ? 16 : 134217728));
			lpStartupInfo.wShowWindow = (short)(visible ? 5 : 0);
			lpStartupInfo.lpDesktop = "winsta0\\default";
			if (!CreateEnvironmentBlock(ref lpEnvironment, phUserToken, bInherit: false))
			{
				throw new Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");
			}
			if (!CreateProcessAsUser(phUserToken, appPath, cmdLine, IntPtr.Zero, IntPtr.Zero, bInheritHandle: false, dwCreationFlags, lpEnvironment, workDir, ref lpStartupInfo, out lpProcessInformation))
			{
				throw new Exception("StartProcessAsCurrentUser: CreateProcessAsUser failed.  Error Code -" + Marshal.GetLastWin32Error());
			}
			uint dwMilliseconds = uint.MaxValue;
			WaitForSingleObject(lpProcessInformation.hProcess, dwMilliseconds);
			int lastWin32Error = Marshal.GetLastWin32Error();
		}
		finally
		{
			CloseHandle(phUserToken);
			if (lpEnvironment != IntPtr.Zero)
			{
				DestroyEnvironmentBlock(lpEnvironment);
			}
			CloseHandle(lpProcessInformation.hThread);
			CloseHandle(lpProcessInformation.hProcess);
		}
		return true;
	}
}
