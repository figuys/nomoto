using System;

namespace lenovo.mbg.service.framework.services;

[Serializable]
public class WindowMessageGeneratedEventArgs : EventArgs
{
	public IntPtr HWnd { get; set; }

	public IntPtr LParam { get; set; }

	public int Msg { get; set; }

	public IntPtr WParam { get; set; }

	public WindowMessageGeneratedEventArgs()
	{
	}

	public WindowMessageGeneratedEventArgs(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		HWnd = hWnd;
		Msg = msg;
		WParam = wparam;
		LParam = LParam;
	}
}
