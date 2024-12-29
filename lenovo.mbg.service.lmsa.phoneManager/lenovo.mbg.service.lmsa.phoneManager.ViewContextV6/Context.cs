using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GoogleAnalytics;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;

public class Context
{
	public static SortedList<ViewType, ViewDescription> ViewList = new SortedList<ViewType, ViewDescription>
	{
		{
			ViewType.START,
			new ViewDescription(typeof(StartViewV6), typeof(StartViewModel))
		},
		{
			ViewType.START_CONNECTING,
			new ViewDescription(typeof(StartViewV6), typeof(StartViewModel))
		},
		{
			ViewType.START_WIFI,
			new ViewDescription(typeof(StartViewV6), typeof(StartViewModel))
		},
		{
			ViewType.START_TUTORIALS,
			new ViewDescription(typeof(StartViewV6), typeof(StartViewModel))
		},
		{
			ViewType.HOME,
			new ViewDescription(typeof(HomeViewV6), typeof(HomeViewModel))
		},
		{
			ViewType.FILE,
			new ViewDescription(typeof(FileViewV7), typeof(FileViewModelV7))
		},
		{
			ViewType.APP,
			new ViewDescription(typeof(AppMgtViewV7), typeof(AppMgtViewModelV7))
		},
		{
			ViewType.PICTURE,
			new ViewDescription(typeof(PICMgtViewV7), typeof(PicMgtViewModelV7))
		},
		{
			ViewType.VIDEO,
			new ViewDescription(typeof(VideoMgtViewV7), typeof(VideoMgtViewModelV7))
		},
		{
			ViewType.MUSIC,
			new ViewDescription(typeof(MusicMgtViewV7), typeof(MusicMgtViewModelV7))
		},
		{
			ViewType.CONTACT,
			new ViewDescription(typeof(ContactMgtViewV6), typeof(ContactViewModelV6))
		},
		{
			ViewType.ONEKEYCLONE,
			new ViewDescription(typeof(OnekeyCloneStartViewV6), typeof(OnekeyCloneStartViewModel))
		},
		{
			ViewType.ONEKEYCLONE_TRANSFER,
			new ViewDescription(typeof(OnekeyCloneTransferViewV6), typeof(OnekeyCloneTransferViewModel))
		},
		{
			ViewType.ONEKEYCLONE_RESULT,
			new ViewDescription(typeof(OnekeyCloneResultViewV6), typeof(OnekeyCloneResultViewModel))
		}
	};

	public static Dictionary<ViewType, BusinessType> ViewType2BusinessType = new Dictionary<ViewType, BusinessType>
	{
		{
			ViewType.HOME,
			BusinessType.HOME
		},
		{
			ViewType.FILE,
			BusinessType.FILE
		},
		{
			ViewType.APP,
			BusinessType.APP
		},
		{
			ViewType.PICTURE,
			BusinessType.PICTURE
		},
		{
			ViewType.VIDEO,
			BusinessType.VIDEO
		},
		{
			ViewType.MUSIC,
			BusinessType.SONG
		},
		{
			ViewType.CONTACT,
			BusinessType.CONTACT
		},
		{
			ViewType.ONEKEYCLONE,
			BusinessType.ONEKEYCLONE
		}
	};

	private static DeviceEx _CurrentDevice;

	protected static ViewType _OneKyCloneSubViewType = ViewType.ONEKEYCLONE;

	protected static bool _IsExecuteWork = false;

	protected static List<ViewType> ActivectedView = new List<ViewType>();

	public static bool IsPluginActived { get; set; }

	public static MainFrameViewModel MainFrame => MainFrameViewModel.Instance;

	public static Tracker Tracker => HostProxy.GoogleAnalyticsTracker.Tracker;

	public static DeviceEx CurrentDevice
	{
		get
		{
			return _CurrentDevice;
		}
		set
		{
			if (value == null || (_CurrentDevice != null && _CurrentDevice.Identifer != value.Identifer))
			{
				OneKyCloneSubViewType = ViewType.ONEKEYCLONE;
				ActivectedView.ForEach(delegate(ViewType n)
				{
					Task.Run(delegate
					{
						FindViewModel<ViewModelBase>(ViewList[n].ViewType)?.Dispose();
					});
				});
				ActivectedView.Clear();
			}
			_CurrentDevice = value;
		}
	}

	public static ViewType CurrentViewType { get; set; }

	public static ViewType OneKyCloneSubViewType
	{
		get
		{
			return _OneKyCloneSubViewType;
		}
		protected set
		{
			_OneKyCloneSubViewType = value;
		}
	}

	public static IMessageBox MessageBox { get; set; }

	public static bool IsExecuteWork
	{
		get
		{
			return _IsExecuteWork;
		}
		set
		{
			_IsExecuteWork = value;
			MainFrame.ChangeNavEnabled(!_IsExecuteWork);
			_ = IsPluginActived;
		}
	}

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
		if (CurrentViewType == viewType && !reload && !reloadData)
		{
			return;
		}
		if (viewType == ViewType.ONEKEYCLONE_TRANSFER || viewType == ViewType.ONEKEYCLONE_RESULT)
		{
			OneKyCloneSubViewType = viewType;
		}
		else if (viewType == ViewType.ONEKEYCLONE)
		{
			OneKyCloneSubViewType = ViewType.ONEKEYCLONE;
			reloadData = true;
		}
		CurrentViewType = viewType;
		IsExecuteWork = CurrentViewType == ViewType.ONEKEYCLONE_TRANSFER;
		if (Thread.CurrentThread.IsBackground)
		{
			Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
		}
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			if (!ActivectedView.Contains(viewType))
			{
				ActivectedView.Add(viewType);
			}
			object currentView = HostProxy.ViewContext.SwitchView(ViewList[viewType], initilizeData, reload, reloadData);
			MainFrame.CurrentView = currentView;
		});
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
