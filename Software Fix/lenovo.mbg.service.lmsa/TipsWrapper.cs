using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa;

public class TipsWrapper
{
	private static object singleInstanceLock = new object();

	private static TipsWrapper singleInstance = null;

	private List<LenovoPopupWindow> mCacheWinList = new List<LenovoPopupWindow>();

	private ReaderWriterLockSlim _rwSync = null;

	public static TipsWrapper SingleInstance
	{
		get
		{
			if (singleInstance == null)
			{
				lock (singleInstanceLock)
				{
					if (singleInstance == null)
					{
						singleInstance = new TipsWrapper();
					}
				}
			}
			return singleInstance;
		}
	}

	private TipsWrapper()
	{
		_rwSync = new ReaderWriterLockSlim();
	}

	public void ShowTips(Window owner, UserControl userControl)
	{
		LenovoPopupWindow hostWindow = GetHostWindow();
		if (hostWindow != null)
		{
			Show(owner, hostWindow, userControl);
		}
	}

	public void ShowTips(Window owner, Type type)
	{
		LenovoPopupWindow hostWindow = GetHostWindow();
		if (hostWindow != null)
		{
			Show(owner, hostWindow, Activator.CreateInstance(type));
		}
	}

	public void CloseTips()
	{
		try
		{
			_rwSync.EnterReadLock();
			foreach (LenovoPopupWindow mCacheWin in mCacheWinList)
			{
				try
				{
					mCacheWin?.Close();
				}
				catch (Exception)
				{
				}
			}
			mCacheWinList.Clear();
		}
		finally
		{
			_rwSync.ExitReadLock();
		}
	}

	private LenovoPopupWindow GetHostWindow()
	{
		LenovoPopupWindow lenovoPopupWindow = null;
		try
		{
			_rwSync.EnterUpgradeableReadLock();
			try
			{
				_rwSync.EnterWriteLock();
				lenovoPopupWindow = new LenovoPopupWindow();
				mCacheWinList.Add(lenovoPopupWindow);
			}
			finally
			{
				_rwSync.ExitWriteLock();
			}
		}
		finally
		{
			_rwSync.ExitUpgradeableReadLock();
		}
		return lenovoPopupWindow;
	}

	private void Show(Window owner, LenovoPopupWindow hostWindow, object content)
	{
		hostWindow.Content = content;
		hostWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		hostWindow.ShowInTaskbar = false;
		HostProxy.HostMaskLayerWrapper.New(hostWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			hostWindow.ShowDialog();
		});
	}
}
