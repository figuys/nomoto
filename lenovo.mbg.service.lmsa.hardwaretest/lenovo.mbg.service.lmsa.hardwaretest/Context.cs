using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hardwaretest.View;
using lenovo.mbg.service.lmsa.hardwaretest.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.hardwaretest;

public class Context
{
	public static SortedList<ViewType, ViewDescription> view = new SortedList<ViewType, ViewDescription>
	{
		{
			ViewType.START,
			new ViewDescription(typeof(StartView), typeof(StartViewModel))
		},
		{
			ViewType.MAIN,
			new ViewDescription(typeof(MainView), typeof(MainViewModel))
		}
	};

	protected static IUserMsgControl Win;

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

	public static DeviceEx CurrentDevice { get; set; }

	public static IMessageBox MessageBox { get; set; }

	public static ViewType CurrentViewType { get; set; }

	public static Dispatcher Dispatcher => Application.Current.Dispatcher;

	public static void Switch(ViewType viewType)
	{
		Switch(viewType, reloadData: false);
	}

	public static void Switch(ViewType viewType, bool reloadData)
	{
		Switch(viewType, null, reload: false, reloadData);
	}

	public static void Switch(ViewType viewType, object initilizeData, bool reload, bool reloadData = false)
	{
		if (CurrentViewType != viewType || reload || reloadData)
		{
			CurrentViewType = viewType;
			if (Thread.CurrentThread.IsBackground)
			{
				Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
			}
			object currentView = HostProxy.ViewContext.SwitchView(view[viewType], initilizeData, reload, reloadData);
			MainFrame.CurrentView = currentView;
			if (CurrentViewType == ViewType.MAIN && CurrentDevice != null && CurrentDevice.ConnectType == ConnectType.Adb)
			{
				MessageBox.ShowMessage("K1608", out Win);
				Win = null;
			}
			else
			{
				Win?.GetMsgUi()?.Close();
				Win?.CloseAction?.Invoke(true);
				Win = null;
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
}
