using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace lenovo.mbg.service.common.utilities;

public class HardwareHelper
{
	public class Com
	{
		public struct DEV_BROADCAST_HDR
		{
			public uint dbch_size;

			public uint dbch_devicetype;

			public uint dbch_reserved;
		}

		protected struct DEV_BROADCAST_PORT_Fixed
		{
			public uint dbcp_size;

			public uint dbcp_devicetype;

			public uint dbcp_reserved;
		}

		public const int WM_DEVICE_CHANGE = 537;

		public const int DBT_DEVICEARRIVAL = 32768;

		public const int DBT_DEVICE_REMOVE_COMPLETE = 32772;

		public const uint DBT_DEVTYP_PORT = 3u;

		public static string GetComPortName(IntPtr wParam, IntPtr lParam)
		{
			if (((DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR))).dbch_devicetype == 3)
			{
				return Marshal.PtrToStringUni(IntPtr.Add(lParam, Marshal.SizeOf(typeof(DEV_BROADCAST_PORT_Fixed))));
			}
			return string.Empty;
		}
	}

	public static string GetHardwareInfo(HardwareEnum hardType, string propKey, string valKeyWords)
	{
		new List<string>();
		ManagementObjectSearcher managementObjectSearcher = null;
		try
		{
			managementObjectSearcher = new ManagementObjectSearcher("select * from " + hardType);
		}
		catch (Exception)
		{
			return string.Empty;
		}
		finally
		{
			if (managementObjectSearcher != null)
			{
				try
				{
					managementObjectSearcher.Dispose();
				}
				catch (Exception)
				{
				}
			}
		}
		return string.Empty;
	}

	public static Rectangle GetPosition(Window win)
	{
		if (win.WindowState != WindowState.Maximized)
		{
			return new Rectangle((int)win.Left, (int)win.Top, (int)win.ActualWidth, (int)win.ActualHeight);
		}
		_ = win.ActualWidth;
		_ = win.ActualHeight;
		return Screen.FromHandle(new WindowInteropHelper(win).Handle).WorkingArea;
	}
}
