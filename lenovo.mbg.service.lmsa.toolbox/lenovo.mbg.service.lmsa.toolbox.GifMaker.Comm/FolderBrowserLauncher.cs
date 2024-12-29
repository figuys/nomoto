using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Comm;

public static class FolderBrowserLauncher
{
	private const string _topLevelSearchString = "Browse For Folder";

	private const int _dlgItemBrowseControl = 0;

	private const int _dlgItemTreeView = 100;

	private const int TV_FIRST = 4352;

	private const int TVM_SELECTITEM = 4363;

	private const int TVM_GETNEXTITEM = 4362;

	private const int TVM_GETITEM = 4364;

	private const int TVM_ENSUREVISIBLE = 4372;

	private const int TVGN_ROOT = 0;

	private const int TVGN_NEXT = 1;

	private const int TVGN_CHILD = 4;

	private const int TVGN_FIRSTVISIBLE = 5;

	private const int TVGN_NEXTVISIBLE = 6;

	private const int TVGN_CARET = 9;

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll")]
	private static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

	public static DialogResult ShowFolderBrowser(FolderBrowserDialog dlg, IWin32Window parent = null)
	{
		DialogResult dialogResult = DialogResult.Cancel;
		int retries = 10;
		Timer t = new Timer();
		try
		{
			t.Tick += delegate
			{
				if (retries > 0)
				{
					int num = retries - 1;
					retries = num;
					IntPtr intPtr = FindWindow(null, "Browse For Folder");
					if (intPtr != IntPtr.Zero)
					{
						IntPtr dlgItem = GetDlgItem(intPtr, 0);
						if (dlgItem != IntPtr.Zero)
						{
							IntPtr dlgItem2 = GetDlgItem(dlgItem, 100);
							if (dlgItem2 != IntPtr.Zero)
							{
								IntPtr intPtr2 = SendMessage(dlgItem2, 4362u, new IntPtr(9), IntPtr.Zero);
								if (intPtr2 != IntPtr.Zero)
								{
									SendMessage(dlgItem2, 4372u, IntPtr.Zero, intPtr2);
									retries = 0;
									t.Stop();
								}
							}
						}
					}
				}
				else
				{
					t.Stop();
					SendKeys.SendWait("{TAB}{TAB}{DOWN}{DOWN}{UP}{UP}");
				}
			};
			t.Interval = 10;
			t.Start();
			return dlg.ShowDialog(parent);
		}
		finally
		{
			if (t != null)
			{
				((IDisposable)t).Dispose();
			}
		}
	}
}
