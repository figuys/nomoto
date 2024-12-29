using System;
using System.Runtime.InteropServices;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;

[StructLayout(LayoutKind.Sequential)]
public class LAMEVersion
{
	public int major;

	public int minor;

	[MarshalAs(UnmanagedType.Bool)]
	public bool alpha;

	[MarshalAs(UnmanagedType.Bool)]
	public bool beta;

	public int psy_major;

	public int psy_minor;

	[MarshalAs(UnmanagedType.Bool)]
	public bool psy_alpha;

	[MarshalAs(UnmanagedType.Bool)]
	public bool psy_beta;

	private IntPtr features_ptr = IntPtr.Zero;

	public string features
	{
		get
		{
			if (features_ptr != IntPtr.Zero)
			{
				return Marshal.PtrToStringAuto(features_ptr);
			}
			return null;
		}
	}
}
