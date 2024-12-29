using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace lenovo.mbg.service.common.utilities;

public static class WebBrowserHelper
{
	[StructLayout(LayoutKind.Explicit, Size = 80)]
	public struct INTERNET_CACHE_ENTRY_INFOA
	{
		[FieldOffset(0)]
		public uint dwStructSize;

		[FieldOffset(4)]
		public IntPtr lpszSourceUrlName;

		[FieldOffset(8)]
		public IntPtr lpszLocalFileName;

		[FieldOffset(12)]
		public uint CacheEntryType;

		[FieldOffset(16)]
		public uint dwUseCount;

		[FieldOffset(20)]
		public uint dwHitRate;

		[FieldOffset(24)]
		public uint dwSizeLow;

		[FieldOffset(28)]
		public uint dwSizeHigh;

		[FieldOffset(32)]
		public System.Runtime.InteropServices.ComTypes.FILETIME LastModifiedTime;

		[FieldOffset(40)]
		public System.Runtime.InteropServices.ComTypes.FILETIME ExpireTime;

		[FieldOffset(48)]
		public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;

		[FieldOffset(56)]
		public System.Runtime.InteropServices.ComTypes.FILETIME LastSyncTime;

		[FieldOffset(64)]
		public IntPtr lpHeaderInfo;

		[FieldOffset(68)]
		public uint dwHeaderInfoSize;

		[FieldOffset(72)]
		public IntPtr lpszFileExtension;

		[FieldOffset(76)]
		public uint dwReserved;

		[FieldOffset(76)]
		public uint dwExemptDelta;
	}

	[DllImport("wininet", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr FindFirstUrlCacheGroup(int dwFlags, int dwFilter, IntPtr lpSearchCondition, int dwSearchCondition, ref long lpGroupId, IntPtr lpReserved);

	[DllImport("wininet", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool FindNextUrlCacheGroup(IntPtr hFind, ref long lpGroupId, IntPtr lpReserved);

	[DllImport("wininet", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool DeleteUrlCacheGroup(long GroupId, int dwFlags, IntPtr lpReserved);

	[DllImport("wininet", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "FindFirstUrlCacheEntryA", SetLastError = true)]
	public static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);

	[DllImport("wininet", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "FindNextUrlCacheEntryA", SetLastError = true)]
	public static extern bool FindNextUrlCacheEntry(IntPtr hFind, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

	[DllImport("wininet", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "DeleteUrlCacheEntryA", SetLastError = true)]
	public static extern bool DeleteUrlCacheEntry(IntPtr lpszUrlName);

	public static void ClearCache()
	{
		long lpGroupId = 0L;
		int lpdwFirstCacheEntryInfoBufferSize = 0;
		int num = 0;
		IntPtr zero = IntPtr.Zero;
		IntPtr zero2 = IntPtr.Zero;
		bool flag = false;
		zero2 = FindFirstUrlCacheGroup(0, 0, IntPtr.Zero, 0, ref lpGroupId, IntPtr.Zero);
		if (zero2 != IntPtr.Zero && 259 == Marshal.GetLastWin32Error())
		{
			return;
		}
		while (259 != Marshal.GetLastWin32Error() && 2 != Marshal.GetLastWin32Error())
		{
			flag = DeleteUrlCacheGroup(lpGroupId, 2, IntPtr.Zero);
			if (!flag && 2 == Marshal.GetLastWin32Error())
			{
				flag = FindNextUrlCacheGroup(zero2, ref lpGroupId, IntPtr.Zero);
			}
			if (!flag && (259 == Marshal.GetLastWin32Error() || 2 == Marshal.GetLastWin32Error()))
			{
				break;
			}
		}
		zero2 = FindFirstUrlCacheEntry(null, IntPtr.Zero, ref lpdwFirstCacheEntryInfoBufferSize);
		if (zero2 != IntPtr.Zero && 259 == Marshal.GetLastWin32Error())
		{
			return;
		}
		num = lpdwFirstCacheEntryInfoBufferSize;
		zero = Marshal.AllocHGlobal(num);
		zero2 = FindFirstUrlCacheEntry(null, zero, ref lpdwFirstCacheEntryInfoBufferSize);
		while (true)
		{
			INTERNET_CACHE_ENTRY_INFOA iNTERNET_CACHE_ENTRY_INFOA = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(zero, typeof(INTERNET_CACHE_ENTRY_INFOA));
			if (259 == Marshal.GetLastWin32Error())
			{
				break;
			}
			lpdwFirstCacheEntryInfoBufferSize = num;
			flag = DeleteUrlCacheEntry(iNTERNET_CACHE_ENTRY_INFOA.lpszSourceUrlName);
			if (!flag)
			{
				flag = FindNextUrlCacheEntry(zero2, zero, ref lpdwFirstCacheEntryInfoBufferSize);
			}
			if (!flag && 259 == Marshal.GetLastWin32Error())
			{
				break;
			}
			if (!flag && lpdwFirstCacheEntryInfoBufferSize > num)
			{
				num = lpdwFirstCacheEntryInfoBufferSize;
				zero = Marshal.ReAllocHGlobal(zero, (IntPtr)num);
				flag = FindNextUrlCacheEntry(zero2, zero, ref lpdwFirstCacheEntryInfoBufferSize);
			}
		}
		Marshal.FreeHGlobal(zero);
	}
}
