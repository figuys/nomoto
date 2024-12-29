using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace lenovo.mbg.service.lmsa;

public class WindowWrapper : System.Windows.Forms.IWin32Window
{
	public IntPtr Handle { get; }

	public WindowWrapper(IntPtr handle)
	{
		Handle = handle;
	}

	public WindowWrapper(Window window)
	{
		Handle = new WindowInteropHelper(window).Handle;
	}
}
