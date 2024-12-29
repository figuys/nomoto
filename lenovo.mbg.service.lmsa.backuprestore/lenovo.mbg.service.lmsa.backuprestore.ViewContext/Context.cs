using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewContext;

public class Context
{
	public static SortedList<ViewType, ViewDescription> view = new SortedList<ViewType, ViewDescription>
	{
		{
			ViewType.START,
			new ViewDescription(typeof(StartView), typeof(StartViewModel))
		},
		{
			ViewType.USBCONNECTHELPER,
			new ViewDescription(typeof(DeviceTutorialsView), typeof(ViewModelBase))
		},
		{
			ViewType.MAIN,
			new ViewDescription(typeof(BackupRestoreMainView), typeof(BackupRestoreMainViewModel))
		},
		{
			ViewType.BACKUPMAIN,
			new ViewDescription(typeof(BackUpMainView), typeof(BackupMainViewModel))
		},
		{
			ViewType.RESTOREMAIN,
			new ViewDescription(typeof(RestoreMainView), typeof(RestoreMainViewModel))
		},
		{
			ViewType.RESTORE,
			new ViewDescription(typeof(RestoreView), typeof(RestoreViewModel))
		}
	};

	private static List<IDisposable> resources = new List<IDisposable>();

	private static readonly object SyncRoot = new object();

	private static Action<IDisposable> InternalDisposeAction = delegate(IDisposable res)
	{
		try
		{
			res.Dispose();
		}
		catch (Exception)
		{
		}
	};

	public static ViewType CurrentRestoreViewType = ViewType.RESTOREMAIN;

	private static MainFrameViewModel _mainFrame;

	private static object locker = new object();

	public static MainFrameViewModel MainFrame
	{
		get
		{
			if (_mainFrame == null)
			{
				lock (locker)
				{
					if (_mainFrame == null)
					{
						_mainFrame = new MainFrameViewModel();
					}
				}
			}
			return _mainFrame;
		}
	}

	public static BackupRestoreMainViewModel Level2Frame { get; set; }

	public static DeviceEx CurrentDevice { get; set; }

	public static IMessageBox MessageBox { get; set; }

	public static ViewType CurrentViewType { get; set; }

	public static void Switch(ViewType viewType, bool level2 = false)
	{
		Switch(viewType, null, reload: false, level2);
	}

	public static void Switch(ViewType viewType, object initilizeData, bool reload, bool level2 = false)
	{
		BackupMainViewModel backupMainViewModel = null;
		RestoreViewModel restoreViewModel = null;
		if (Level2Frame != null && Level2Frame.CurrentView != null && Level2Frame.CurrentView is BackUpMainView && (Level2Frame.CurrentView as BackUpMainView).DataContext is BackupMainViewModel)
		{
			backupMainViewModel = (Level2Frame.CurrentView as BackUpMainView).DataContext as BackupMainViewModel;
		}
		if (Level2Frame != null && Level2Frame.CurrentView != null && Level2Frame.CurrentView is RestoreView && (Level2Frame.CurrentView as RestoreView).DataContext is RestoreViewModel)
		{
			restoreViewModel = (Level2Frame.CurrentView as RestoreView).DataContext as RestoreViewModel;
		}
		if ((backupMainViewModel != null && backupMainViewModel.VM != null && backupMainViewModel.VM.IsRunning.HasValue) || (restoreViewModel != null && restoreViewModel.VM != null && restoreViewModel.VM.IsRunning.HasValue))
		{
			Level2Frame.TitleBarlVisible = Visibility.Collapsed;
		}
		else if (CurrentViewType != viewType)
		{
			CurrentViewType = viewType;
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				MainFrame.BackVisibility = ((CurrentViewType != ViewType.USBCONNECTHELPER) ? Visibility.Collapsed : Visibility.Visible);
				MainFrame.BottomLineVisibility = ((CurrentViewType == ViewType.START) ? Visibility.Collapsed : Visibility.Visible);
			});
			object currentView = HostProxy.ViewContext.SwitchView(view[viewType], initilizeData, reload);
			if (level2)
			{
				Level2Frame.CurrentView = currentView;
			}
			else
			{
				MainFrame.CurrentView = currentView;
			}
		}
	}

	public static FrameworkElement FindView(Type viewType)
	{
		return HostProxy.ViewContext.FindView(viewType);
	}

	public static TViewModel FindViewModel<TViewModel>(Type viewType, string uiid = null) where TViewModel : IViewModelBase
	{
		return HostProxy.ViewContext.FindViewModel<TViewModel>(viewType, uiid);
	}

	public static void RegisterWorker(IDisposable resource)
	{
		lock (SyncRoot)
		{
			if (!resources.Contains(resource))
			{
				resources.Add(resource);
			}
		}
	}

	public static void RemoveWorker(IDisposable resource)
	{
		lock (SyncRoot)
		{
			if (resources.Contains(resource))
			{
				resources.Remove(resource);
			}
		}
	}

	public static void DisposeWorker()
	{
		List<IDisposable> list = null;
		lock (SyncRoot)
		{
			list = resources.ToList();
			resources.Clear();
		}
		foreach (IDisposable item in list)
		{
			InternalDisposeAction.BeginInvoke(item, delegate(IAsyncResult callbackAR)
			{
				InternalDisposeAction.EndInvoke(callbackAR);
			}, null);
		}
	}
}
